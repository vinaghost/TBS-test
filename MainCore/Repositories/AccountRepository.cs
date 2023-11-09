using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Repositories
{
    [RegisterAsTransient]
    public class AccountRepository : IAccountRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IUseragentManager _useragentManager;

        public AccountRepository(IDbContextFactory<AppDbContext> contextFactory, IUseragentManager useragentManager)
        {
            _contextFactory = contextFactory;
            _useragentManager = useragentManager;
        }

        public AccountDto Get(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts
                .AsNoTracking()
                .Where(x => x.Id == accountId.Value)
                .ToDto()
                .FirstOrDefault();
            return account;
        }

        public AccessDto GetAccess(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var access = context.Accesses
               .AsNoTracking()
               .Where(x => x.AccountId == accountId.Value)
               .OrderBy(x => x.LastUsed) // get oldest one
               .ToDto()
               .FirstOrDefault();
            return access;
        }

        public string GetUsername(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var username = context.Accounts
                .Where(x => x.Id == accountId.Value)
                .Select(x => x.Username)
                .FirstOrDefault();
            return username;
        }

        public string GetPassword(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var password = context.Accesses
                .Where(x => x.AccountId == accountId.Value)
                .OrderByDescending(x => x.LastUsed)
                .Select(x => x.Password)
                .FirstOrDefault();
            return password;
        }

        public bool Add(AccountDto dto)
        {
            using var context = _contextFactory.CreateDbContext();

            var isExist = context.Accounts
                .AsNoTracking()
                .Where(x => x.Username == dto.Username)
                .Where(x => x.Server == dto.Server)
                .Any();

            if (isExist) return false;

            var account = dto.ToEntity();
            foreach (var access in account.Accesses)
            {
                if (string.IsNullOrEmpty(access.Useragent))
                {
                    access.Useragent = _useragentManager.Get();
                }
            }
            context.Add(account);
            context.SaveChanges();
            context.FillAccountSettings(new(account.Id));
            return true;
        }

        public void Add(List<AccountDetailDto> dtos)
        {
            using var context = _contextFactory.CreateDbContext();

            var accounts = new List<Account>();
            foreach (var dto in dtos)
            {
                var isExist = context.Accounts
                    .AsNoTracking()
                    .Where(x => x.Username == dto.Username)
                    .Where(x => x.Server == dto.Server)
                    .Any();
                if (isExist) continue;
                var account = dto.ToEnitty();
                foreach (var access in account.Accesses)
                {
                    if (string.IsNullOrEmpty(access.Useragent))
                    {
                        access.Useragent = _useragentManager.Get();
                    }
                }
                accounts.Add(account);
            }
            context.AddRange(accounts);
            context.SaveChanges();

            foreach (var account in accounts)
            {
                context.FillAccountSettings(new(account.Id));
            }
        }

        public void Delete(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Accounts
                .Where(x => x.Id == accountId.Value)
                .ExecuteDelete();
        }

        public void Update(AccountDto dto)
        {
            using var context = _contextFactory.CreateDbContext();

            var account = dto.ToEntity();
            foreach (var access in account.Accesses)
            {
                if (string.IsNullOrEmpty(access.Useragent))
                {
                    access.Useragent = _useragentManager.Get();
                }
            }
            context.Update(account);
            context.SaveChanges();
        }
    }
}