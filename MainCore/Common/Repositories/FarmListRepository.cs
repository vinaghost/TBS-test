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

        public FarmListRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public List<FarmList> GetList(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var farmLists = context.FarmLists
                    .Where(x => x.AccountId == accountId)
                    .ToList();
            return farmLists;
        }

        public void ActiveFarmList(int farmListId)
        {
            using var context = _contextFactory.CreateDbContext();
            context.FarmLists
               .Where(x => x.Id == farmListId)
               .ExecuteUpdate(x => x.SetProperty(x => x.IsActive, x => !x.IsActive));
        }

        public int CountActiveFarmLists(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var count = context.FarmLists
                    .Where(x => x.AccountId == accountId)
                    .Where(x => x.IsActive)
                    .Count();
            return count;
        }

        public List<int> GetActiveFarmLists(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var farmListIds = context.FarmLists
                    .Where(x => x.AccountId == accountId)
                    .Where(x => x.IsActive)
                    .Select(x => x.Id)
                    .ToList();
            return farmListIds;
        }

        public void Update(int accountId, List<FarmList> farmLists)
        {
            using var context = _contextFactory.CreateDbContext();

            var dbFarmList = context.FarmLists.Where(x => x.AccountId == accountId).ToList();

            var newFarmList = farmLists.Except(dbFarmList).ToList();
            var oldFarmList = dbFarmList.Except(farmLists).ToList();
            var updateFarmList = dbFarmList.Where(x => !oldFarmList.Contains(x)).ToList();

            context.AddRange(newFarmList);
            context.RemoveRange(oldFarmList);
            foreach (var village in updateFarmList)
            {
                var vill = farmLists.FirstOrDefault(x => x.Id == village.Id);
                if (vill is null) break;

                village.Name = vill.Name;
            }
            context.UpdateRange(updateFarmList);

            context.SaveChanges();
        }
    }
}