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
        private readonly AppDbContext _context;

        public FarmListRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<FarmListDto> GetList(int accountId)
        {
            var farmLists = _context.FarmLists
                    .Where(x => x.AccountId == accountId)
                    .ProjectToDto()
                    .AsEnumerable();
            return farmLists;
        }

        public void ActiveFarmList(int farmListId)
        {
            _context.FarmLists
               .Where(x => x.Id == farmListId)
               .ExecuteUpdate(x => x.SetProperty(x => x.IsActive, x => !x.IsActive));
        }

        public int CountActiveFarmLists(int accountId)
        {
            var count = _context.FarmLists
                    .Where(x => x.AccountId == accountId)
                    .Where(x => x.IsActive)
                    .Count();
            return count;
        }

        public List<int> GetActiveFarmLists(int accountId)
        {
            var farmListIds = _context.FarmLists
                    .Where(x => x.AccountId == accountId)
                    .Where(x => x.IsActive)
                    .Select(x => x.Id)
                    .ToList();
            return farmListIds;
        }

        public void Update(int accountId, List<FarmList> farmLists)
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