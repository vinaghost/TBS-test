using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Common.Repositories
{
    [RegisterAsTransient]
    public class VillageRepository : IVillageRepository
    {
        private readonly AppDbContext _context;

        private readonly IVillageSettingRepository _villageSettingRepository;

        public VillageRepository(AppDbContext context, IVillageSettingRepository villageSettingRepository)
        {
            _context = context;
            _villageSettingRepository = villageSettingRepository;
        }

        public int GetActive(int accountId)
        {
            var village = _context.Villages
                .Where(x => x.AccountId == accountId && x.IsActive)
                .Select(x => x.Id)
                .FirstOrDefault();
            return village;
        }

        public List<int> GetInactive(int accountId)
        {
            var villages = _context.Villages
                .Where(x => x.AccountId == accountId && !x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => x.Id)
                .ToList();
            return villages;
        }

        public Village Get(int villageId)
        {
            return _context.Villages.Find(villageId);
        }

        public List<VillageDto> GetList(int accountId)
        {
            return _context.Villages
                .Where(x => x.AccountId == accountId)
                .OrderBy(x => x.Name)
                .ProjectToDto()
                .ToList();
        }

        public List<int> GetUnloadList(int accountId)
        {
            var villages = _context.Villages
                .Where(x => x.AccountId == accountId)
                .Include(x => x.Buildings)
                .Where(x => x.Buildings.Count < 19)
                .OrderBy(x => x.Name)
                .Select(x => x.Id)
                .ToList();
            return villages;
        }

        public List<Village> Update(int accountId, List<Village> villages)
        {
            List<Village> newVillages;

            var villagesOnDb = _context.Villages.Where(x => x.AccountId == accountId).ToList();

            newVillages = villages.Except(villagesOnDb).ToList();
            var oldVillages = villagesOnDb.Except(villages).ToList();
            var updateVillages = villagesOnDb.Where(x => !oldVillages.Contains(x)).ToList();

            _context.AddRange(newVillages);
            _context.RemoveRange(oldVillages);
            foreach (var village in updateVillages)
            {
                var vill = villages.FirstOrDefault(x => x.Id == village.Id);
                if (vill is null) break;

                village.Name = vill.Name;
                village.IsActive = vill.IsActive;
                village.IsUnderAttack = vill.IsUnderAttack;
            }
            _context.UpdateRange(updateVillages);

            _context.SaveChanges();

            foreach (var village in newVillages)
            {
                _villageSettingRepository.CheckSetting(_context, village.Id);
            }

            return newVillages;
        }
    }
}