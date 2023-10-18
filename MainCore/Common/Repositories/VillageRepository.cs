using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Common.Repositories
{
    [RegisterAsSingleton]
    public class VillageRepository : IVillageRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        private readonly IVillageSettingRepository _villageSettingRepository;

        public VillageRepository(IDbContextFactory<AppDbContext> contextFactory, IVillageSettingRepository villageSettingRepository)
        {
            _contextFactory = contextFactory;
            _villageSettingRepository = villageSettingRepository;
        }

        public int GetActive(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var village = context.Villages
                .Where(x => x.AccountId == accountId && x.IsActive)
                .Select(x => x.Id)
                .FirstOrDefault();
            return village;
        }

        public List<int> GetInactive(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages
                .Where(x => x.AccountId == accountId && !x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => x.Id)
                .ToList();
            return villages;
        }

        public Village Get(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Villages.Find(villageId);
        }

        public List<VillageDto> GetList(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Villages
                .Where(x => x.AccountId == accountId)
                .OrderBy(x => x.Name)
                .ProjectToDto()
                .ToList();
        }

        public List<int> GetUnloadList(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages
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
            using var context = _contextFactory.CreateDbContext();

            var villagesOnDb = context.Villages.Where(x => x.AccountId == accountId).ToList();

            newVillages = villages.Except(villagesOnDb).ToList();
            var oldVillages = villagesOnDb.Except(villages).ToList();
            var updateVillages = villagesOnDb.Where(x => !oldVillages.Contains(x)).ToList();

            context.AddRange(newVillages);
            context.RemoveRange(oldVillages);
            foreach (var village in updateVillages)
            {
                var vill = villages.FirstOrDefault(x => x.Id == village.Id);
                if (vill is null) break;

                village.Name = vill.Name;
                village.IsActive = vill.IsActive;
                village.IsUnderAttack = vill.IsUnderAttack;
            }
            context.UpdateRange(updateVillages);

            context.SaveChanges();

            foreach (var village in newVillages)
            {
                _villageSettingRepository.CheckSetting(context, village.Id);
            }

            return newVillages;
        }
    }
}