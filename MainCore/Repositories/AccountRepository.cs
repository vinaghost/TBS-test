using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Repositories
{
    [RegisterAsTransient]
    public class AccountRepository : IAccountRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public AccountRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public string GetUsernameById(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var username = context.Accounts
                .Where(x => x.Id == accountId.Value)
                .Select(x => x.Username)
                .FirstOrDefault();
            return username;
        }

        public string GetPasswordById(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var password = context.Accesses
                .Where(x => x.AccountId == accountId.Value)
                .OrderByDescending(x => x.LastUsed)
                .Select(x => x.Password)
                .FirstOrDefault();
            return password;
        }
    }
}