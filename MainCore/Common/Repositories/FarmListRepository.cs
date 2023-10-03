using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Common.Repositories
{
    [RegisterAsSingleton]
    public class FarmListRepository : IFarmListRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public event Func<int, Task> FarmListsUpdated;

        public FarmListRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<FarmList>> GetList(int accountId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var farmListIds = await context.FarmLists
                    .Where(x => x.AccountId == accountId)
                    .ToListAsync();
            return farmListIds;
        }

        public async Task ActiveFarmList(int farmListId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            await context.FarmLists
                .Where(x => x.Id == farmListId)
                .ExecuteUpdateAsync(x => x.SetProperty(x => x.IsActive, x => !x.IsActive));
        }

        public async Task<int> CountActiveFarmLists(int accountId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var count = await context.FarmLists
                    .Where(x => x.AccountId == accountId)
                    .Where(x => x.IsActive)
                    .CountAsync();
            return count;
        }

        public async Task<List<int>> GetActiveFarmLists(int accountId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var farmListIds = await context.FarmLists
                    .Where(x => x.AccountId == accountId)
                    .Where(x => x.IsActive)
                    .Select(x => x.Id)
                    .ToListAsync();
            return farmListIds;
        }

        public async Task Update(int accountId, List<FarmList> farmLists)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var dbFarmList = await context.FarmLists.Where(x => x.AccountId == accountId).ToListAsync();

                var newFarmList = farmLists.Except(dbFarmList).ToList();
                var oldFarmList = dbFarmList.Except(farmLists).ToList();
                var updateFarmList = dbFarmList.Where(x => !oldFarmList.Contains(x)).ToList();

                await context.AddRangeAsync(newFarmList);
                context.RemoveRange(oldFarmList);
                foreach (var village in updateFarmList)
                {
                    var vill = farmLists.FirstOrDefault(x => x.Id == village.Id);
                    if (vill is null) break;

                    village.Name = vill.Name;
                }
                context.UpdateRange(updateFarmList);

                await context.SaveChangesAsync();
            }
            if (FarmListsUpdated is not null)
            {
                await FarmListsUpdated(accountId);
            }
        }
    }
}