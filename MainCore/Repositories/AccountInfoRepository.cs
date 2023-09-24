using MainCore.Models;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Repositories
{
    public class AccountInfoRepository : IAccountInfoRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public AccountInfoRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
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