using FluentResults;
using MainCore.Common.Errors.Database;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Common.Repositories
{
    [RegisterAsSingleton]
    public class AccountRepository : IAccountRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IUseragentManager _useragentManager;
        private readonly IAccountSettingRepository _accountSettingRepository;

        public AccountRepository(IDbContextFactory<AppDbContext> contextFactory, IUseragentManager useragentManager, MainCore.Common.Repositories.IAccountSettingRepository accountSettingRepository)
        {
            _contextFactory = contextFactory;
            _useragentManager = useragentManager;
            _accountSettingRepository = accountSettingRepository;
        }

        public Result Add(AccountDto dto)
        {
            var mapper = new AccountMapper();
            var account = mapper.Map(dto);
            return Add(account);
        }

        public Result Add(Account account)
        {
            using var context = _contextFactory.CreateDbContext();

            var query = context.Accounts
                .Where(x => x.Username == account.Username
                            && x.Server == account.Server);

            if (query.Any()) return Result.Fail(new AccountExist(account.Username, account.Server));
            foreach (var access in account.Accesses)
            {
                access.Useragent = _useragentManager.Get();
            }
            context.Add(account);
            context.SaveChanges();

            _accountSettingRepository.CheckSetting(context, account.Id);

            return Result.Ok();
        }

        public void AddRange(List<AccountDto> dtos)
        {
            var mapper = new AccountMapper();
            var accounts = mapper.Map(dtos);
            AddRange(accounts);
        }

        public void AddRange(List<AccountsDto> dtos)
        {
            var mapper = new AccountsMapper();
            var accounts = mapper.Map(dtos);
            AddRange(accounts);
        }

        public void AddRange(List<Account> accounts)
        {
            using var context = _contextFactory.CreateDbContext();

            foreach (var account in accounts)
            {
                foreach (var access in account.Accesses)
                {
                    access.Useragent = _useragentManager.Get();
                }
            }
            context.AddRange(accounts);
            context.SaveChanges();
            foreach (var account in accounts)
            {
                _accountSettingRepository.CheckSetting(context, account.Id);
            }
        }

        public List<AccountDto> Get()
        {
            using var context = _contextFactory.CreateDbContext();
            var accounts = context.Accounts
                .AsQueryable()
                .ProjectToDto()
                .ToList();
            return accounts;
        }

        public AccountDto Get(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts
                .Find(accountId);
            context.Entry(account)
               .Collection(x => x.Accesses)
               .Load();
            var mapper = new AccountMapper();
            return mapper.Map(account);
        }

        public void Edit(AccountDto dto)
        {
            var mapper = new AccountMapper();
            var account = mapper.Map(dto);
            Edit(account);
        }

        public void Edit(Account account)
        {
            using var context = _contextFactory.CreateDbContext();

            var accessIds = account.Accesses
                .Where(x => x.AccountId == account.Id)
                .Select(x => x.Id)
                .ToList();

            var oldAccessIds = accessIds
                .Except(account.Accesses.Select(x => x.Id));

            context.Accesses
               .Where(x => oldAccessIds.Contains(x.Id))
               .ExecuteDelete();

            context.Update(account);
            context.SaveChanges();
        }

        public void Delete(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();

            context.Accounts
               .Where(x => x.Id == accountId)
               .ExecuteDelete();
        }
    }
}