using FluentResults;
using MainCore.Common.Errors.Database;
using MainCore.Common.Notification;
using MainCore.DTO;
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
        private readonly AppDbContext _context;
        private readonly IUseragentManager _useragentManager;
        private readonly IMediator _mediator;

        public AccountRepository(AppDbContext context, IUseragentManager useragentManager, IMediator mediator)
        {
            _context = context;
            _useragentManager = useragentManager;
            _mediator = mediator;
        }

        public async Task<Result> Add(AccountDto dto)
        {
            var isExist = await Task.Run(() =>
                _context.Accounts
                    .Where(x => x.Username == dto.Username
                                && x.Server == dto.Server)
                    .Any());

            if (isExist) return Result.Fail(new AccountExist(dto.Username, dto.Server));
            foreach (var access in dto.Accesses)
            {
                access.Useragent = _useragentManager.Get();
            }

            var mapper = new AccountMapper();
            var entity = mapper.Map(dto);

            _context.Add(entity);
            await Task.Run(() => _context.SaveChanges());
            await _mediator.Publish(new AccountUpdated());
            return Result.Ok();
        }

        public async Task AddRange(IEnumerable<AccountDto> dtos)
        {
            var mapper = new AccountMapper();
            foreach (var dto in dtos)
            {
                var isExist = await Task.Run(() =>
                    _context.Accounts
                        .Where(x => x.Username == dto.Username
                                    && x.Server == dto.Server)
                        .Any());

                if (isExist) continue;

                foreach (var access in dto.Accesses)
                {
                    access.Useragent = _useragentManager.Get();
                }
                var entity = mapper.Map(dto);
                _context.Add(entity);
            }

            await Task.Run(() => _context.SaveChanges());
            await _mediator.Publish(new AccountUpdated());
        }

        public async Task<IEnumerable<AccountDto>> GetAll()
        {
            var accounts = await Task.Run(() =>
                _context.Accounts
                    .AsQueryable()
                    .ProjectToDto()
                    .AsEnumerable());
            return accounts;
        }

        public async Task<AccountDto> GetById(int accountId)
        {
            var account = await Task.Run(() =>
                _context.Accounts
                    .Find(accountId));

            await Task.Run(() =>
                _context.Entry(account)
                    .Collection(x => x.Accesses)
                    .Load());

            var mapper = new AccountMapper();
            var dto = mapper.Map(account);
            return dto;
        }

        public async Task Edit(AccountDto dto)
        {
            _context.Accesses
               .Where(x => x.AccountId == dto.Id)
               .ExecuteDelete();

            var mapper = new AccountMapper();
            var account = mapper.Map(dto);

            _context.Update(account);
            await Task.Run(() => _context.SaveChanges());

            await _mediator.Publish(new AccountUpdated());
        }

        public async Task DeleteById(int accountId)
        {
            await Task.Run(() =>
                _context.Accounts
                    .Where(x => x.Id == accountId)
                    .ExecuteDelete());

            await _mediator.Publish(new AccountUpdated());
        }
    }
}