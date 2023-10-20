using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Common.Repositories
{
    [RegisterAsSingleton]
    public class AccountInfoRepository : IAccountInfoRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public AccountInfoRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public bool IsPlusActive(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var accountInfo = context.AccountsInfo
                .FirstOrDefault(x => x.AccountId == accountId);
            if (accountInfo is null) return false;
            return accountInfo.HasPlusAccount;
        }

        public void Update(int accountId, AccountInfo accountInfo)
        {
            using var context = _contextFactory.CreateDbContext();

            var dbAccountInfo = context.AccountsInfo
                .FirstOrDefault(x => x.AccountId == accountId);
            if (dbAccountInfo is null)
            {
                context.Add(accountInfo);
            }
            else
            {
                dbAccountInfo.Tribe = accountInfo.Tribe;
                dbAccountInfo.Gold = accountInfo.Gold;
                dbAccountInfo.Silver = accountInfo.Silver;
                dbAccountInfo.HasPlusAccount = accountInfo.HasPlusAccount;
                context.Update(dbAccountInfo);
            }
            context.SaveChanges();
        }
    }
}