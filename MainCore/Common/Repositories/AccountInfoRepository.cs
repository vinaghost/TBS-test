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

        public async Task<bool> IsPlusActive(int accountId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var accountInfo = await context.AccountsInfo.FirstOrDefaultAsync(x => x.AccountId == accountId);
            return accountInfo?.HasPlusAccount ?? false;
        }

        public async Task Update(int accountId, AccountInfo accountInfo)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var dbAccountInfo = await context.AccountsInfo.FirstOrDefaultAsync(x => x.AccountId == accountId);
            if (dbAccountInfo is null)
            {
                await context.AddAsync(accountInfo);
            }
            else
            {
                dbAccountInfo.Tribe = accountInfo.Tribe;
                dbAccountInfo.Gold = accountInfo.Gold;
                dbAccountInfo.Silver = accountInfo.Silver;
                dbAccountInfo.HasPlusAccount = accountInfo.HasPlusAccount;
                context.Update(dbAccountInfo);
            }
            await context.SaveChangesAsync();
        }
    }
}