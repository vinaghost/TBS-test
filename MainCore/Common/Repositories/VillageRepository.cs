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

        public event Func<int, Task> VillageListChanged;

        private readonly IVillageSettingRepository _villageSettingRepository;

        public VillageRepository(IDbContextFactory<AppDbContext> contextFactory, IVillageSettingRepository villageSettingRepository)
        {
            _contextFactory = contextFactory;
            _villageSettingRepository = villageSettingRepository;
        }

        public async Task<Village> GetActive(int accountId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var village = await context.Villages
                                        .FirstOrDefaultAsync(x => x.AccountId == accountId && x.IsActive);
            return village;
        }

        public async Task<List<Village>> GetInactive(int accountId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var villages = await context.Villages
                                        .Where(x => x.AccountId == accountId && !x.IsActive)
                                        .OrderBy(x => x.Name)
                                        .ToListAsync();
            return villages;
        }

        public async Task<Village> Get(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Villages.FindAsync(villageId);
        }

        public async Task<List<Village>> GetList(int accountId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Villages
                .Where(x => x.AccountId == accountId)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<Village>> GetUnloadList(int accountId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var villages = await context.Villages
                .Where(x => x.AccountId == accountId)
                .Include(x => x.Buildings)
                .Where(x => x.Buildings.Count < 38)
                .OrderBy(x => x.Name)
                .ToListAsync();
            return villages;
        }

        public async Task<List<Village>> Update(int accountId, List<Village> villages)
        {
            List<Village> newVillages;
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var villagesOnDb = await context.Villages.Where(x => x.AccountId == accountId).ToListAsync();

                newVillages = villages.Except(villagesOnDb).ToList();
                var oldVillages = villagesOnDb.Except(villages).ToList();
                var updateVillages = villagesOnDb.Where(x => !oldVillages.Contains(x)).ToList();

                await context.AddRangeAsync(newVillages);
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

                await context.SaveChangesAsync();

                foreach (var village in newVillages)
                {
                    await _villageSettingRepository.CheckSetting(village.Id, context);
                }
            }
            if (VillageListChanged is not null)
            {
                await VillageListChanged(accountId);
            }
            return newVillages;
        }
    }
}