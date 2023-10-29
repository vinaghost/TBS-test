using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Common.Repositories
{
    [RegisterAsTransient]
    public class FarmListRepository : IFarmListRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public FarmListRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public IEnumerable<FarmListDto> GetList(AccountId accountId)
        {
            var farmLists = _context.FarmLists
                    .Where(x => x.AccountId == accountId)
                    .ProjectToDto()
                    .AsEnumerable();
            return farmLists;
        }

        public void ActiveFarmList(FarmListId farmListId)
        {
            _context.FarmLists
               .Where(x => x.Id == farmListId)
               .ExecuteUpdate(x => x.SetProperty(x => x.IsActive, x => !x.IsActive));
        }

        public int CountActiveFarmLists(AccountId accountId)
        {
            var count = _context.FarmLists
                    .Where(x => x.AccountId == accountId)
                    .Where(x => x.IsActive)
                    .Count();
            return count;
        }

        public List<FarmListId> GetActiveFarmLists(AccountId accountId)
        {
            var farmListIds = _context.FarmLists
                    .Where(x => x.AccountId == accountId)
                    .Where(x => x.IsActive)
                    .Select(x => x.Id)
                    .ToList();
            return farmListIds;
        }

        public void Update(AccountId accountId, List<FarmList> farmLists)
        {
            var dbFarmList = _context.FarmLists.Where(x => x.AccountId == accountId).ToList();

            var newFarmList = farmLists.Except(dbFarmList).ToList();
            var oldFarmList = dbFarmList.Except(farmLists).ToList();
            var updateFarmList = dbFarmList.Where(x => !oldFarmList.Contains(x)).ToList();

            _context.AddRange(newFarmList);
            _context.RemoveRange(oldFarmList);
            foreach (var village in updateFarmList)
            {
                var vill = farmLists.FirstOrDefault(x => x.Id == village.Id);
                if (vill is null) break;

                village.Name = vill.Name;
            }
            _context.UpdateRange(updateFarmList);

            _context.SaveChanges();
        }
    }
}