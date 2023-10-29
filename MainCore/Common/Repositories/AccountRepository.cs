using MainCore.Common.Notification;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Common.Repositories
{
    [RegisterAsTransient]
    public class AccountRepository : IAccountRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IUseragentManager _useragentManager;
        private readonly IMediator _mediator;

        public AccountRepository(IDbContextFactory<AppDbContext> contextFactory, IUseragentManager useragentManager, IMediator mediator)
        {
            _contextFactory = contextFactory;
            _useragentManager = useragentManager;
            _mediator = mediator;
        }

        public async Task<bool> IsExist(AccountDto dto)
        {
            using var context = _contextFactory.CreateDbContext();
            var isExist = await Task.Run(() =>
                    context.Accounts
                        .Where(x => x.Username == dto.Username
                                    && x.Server == dto.Server)
                        .Any());
            return isExist;
        }

        public async Task Add(AccountDto dto)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var mapper = new AccountMapper();
                var entity = mapper.Map(dto);
                foreach (var access in entity.Accesses)
                {
                    access.Useragent = _useragentManager.Get();
                }
                context.Add(entity);
                await Task.Run(context.SaveChanges);
                dto.Id = entity.Id;
            }
            await _mediator.Publish(new AccountUpdated());
        }

        public async Task<List<AccountDto>> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            var accounts = await Task.Run(
                context.Accounts
                    .AsQueryable()
                    .ProjectToDto()
                    .ToList);
            return accounts;
        }

        public async Task<AccountDto> GetById(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var account = await Task.Run(() =>
                context.Accounts
                    .Find(accountId));

            await Task.Run(() =>
            {
                context.Entry(account)
                    .Collection(x => x.Accesses)
                    .Load();
            });

            var mapper = new AccountMapper();
            var dto = mapper.Map(account);
            return dto;
        }

        public async Task<string> GetUsernameById(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var username = await Task.Run(() =>
               context.Accounts
                   .Where(x => x.Id == accountId)
                   .Select(x => x.Username)
                   .FirstOrDefault());
            return username;
        }

        public async Task<string> GetPasswordById(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var password = await Task.Run(() =>
               context.Accesses
                   .Where(x => x.AccountId == accountId)
                   .OrderByDescending(x => x.LastUsed)
                   .Select(x => x.Password)
                   .FirstOrDefault());
            return password;
        }

        public async Task Edit(AccountDto dto)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var mapper = new AccountMapper();
                var account = mapper.Map(dto);
                foreach (var access in account.Accesses)
                {
                    if (string.IsNullOrEmpty(access.Useragent))
                    {
                        access.Useragent = _useragentManager.Get();
                    }
                }
                context.Update(account);
                await Task.Run(context.SaveChanges);
            }
            await _mediator.Publish(new AccountUpdated());
        }

        public async Task DeleteById(AccountId accountId)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                await Task.Run(() =>
                    context.Accounts
                        .Where(x => x.Id == accountId)
                        .ExecuteDelete());
            }
            await _mediator.Publish(new AccountUpdated());
        }
    }
}