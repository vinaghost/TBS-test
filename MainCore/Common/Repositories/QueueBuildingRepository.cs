using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Features.InstantUpgrade.Tasks;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Common.Repositories
{
    [RegisterAsSingleton]
    public class QueueBuildingRepository : IQueueBuildingRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IVillageSettingRepository _villageSettingRepository;
        private readonly IAccountInfoRepository _accountInfoRepository;
        private readonly ITaskManager _taskManager;

        public event Func<int, Task> QueueBuildingUpdated;

        public QueueBuildingRepository(IDbContextFactory<AppDbContext> contextFactory, IVillageSettingRepository villageSettingRepository, IAccountInfoRepository accountInfoRepository, ITaskManager taskManager)
        {
            _contextFactory = contextFactory;
            _villageSettingRepository = villageSettingRepository;
            _accountInfoRepository = accountInfoRepository;
            _taskManager = taskManager;
        }

        public async Task<QueueBuilding> GetFirst(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var queueBuildings = context.QueueBuildings
                .Where(x => x.VillageId == villageId && x.Type != BuildingEnums.Site)
                .OrderBy(x => x.Position);
            var queueBuilding = await queueBuildings.FirstOrDefaultAsync();
            return queueBuilding;
        }

        public async Task<List<QueueBuilding>> GetList(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var queueBuildings = context.QueueBuildings
                .Where(x => x.VillageId == villageId && x.Type != BuildingEnums.Site)
                .OrderBy(x => x.Position);
            return await queueBuildings.ToListAsync();
        }

        public async Task Update(int villageId, List<Building> buildings)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var queueBuildings = context.QueueBuildings
                    .Where(x => x.VillageId == villageId && x.Type != BuildingEnums.Site);

                if (buildings.Count == 1)
                {
                    var building = buildings[0];
                    queueBuildings = queueBuildings.Where(x => x.Type == building.Type);
                    var list = await queueBuildings.ToListAsync();
                    foreach (var item in list)
                    {
                        item.Location = building.Location;
                    }
                    context.UpdateRange(list);
                }
                else if (buildings.Count == 2)
                {
                    foreach (var building in buildings)
                    {
                        var queueBuilding = await queueBuildings.FirstOrDefaultAsync(x => x.Type == building.Type);
                        queueBuilding.Location = building.Location;
                        context.Update(queueBuilding);
                    }
                }
                await context.SaveChangesAsync();
            }
            await TriggerCallback(villageId);
        }

        public async Task Update(int villageId, List<QueueBuilding> queueBuildings)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var dbQueueBuildings = context.QueueBuildings.Where(x => x.VillageId == villageId);

                for (var i = 0; i < queueBuildings.Count; i++)
                {
                    var queueBuilding = queueBuildings[i];
                    var dbQueueBuilding = await dbQueueBuildings.FirstOrDefaultAsync(x => x.Position == i);
                    if (dbQueueBuilding is null)
                    {
                        await context.AddAsync(queueBuilding);
                    }
                    else
                    {
                        dbQueueBuilding.Type = queueBuilding.Type;
                        dbQueueBuilding.Level = queueBuilding.Level;
                        dbQueueBuilding.CompleteTime = queueBuilding.CompleteTime;
                        context.Update(dbQueueBuilding);
                    }
                }
                await context.SaveChangesAsync();
            }
            await TriggerCallback(villageId);
        }

        private async Task TriggerCallback(int villageId)
        {
            if (QueueBuildingUpdated is not null)
            {
                await QueueBuildingUpdated(villageId);
            }

            await TriggerInstantUpgrade(villageId);
        }

        private async Task TriggerInstantUpgrade(int villageId)
        {
            var instantUpgrade = await _villageSettingRepository.GetBoolSetting(villageId, VillageSettingEnums.InstantUpgrade);
            if (instantUpgrade)
            {
                var applyRomanQueueLogicWhenBuilding = await _villageSettingRepository.GetBoolSetting(villageId, VillageSettingEnums.ApplyRomanQueueLogicWhenBuilding);
                int accountId, count;
                using (var context = await _contextFactory.CreateDbContextAsync())
                {
                    var village = await context.Villages.FindAsync(villageId);
                    accountId = village.AccountId;
                    count = await context.QueueBuildings.Where(x => x.VillageId == villageId).CountAsync();
                }

                var isPlusActive = await _accountInfoRepository.IsPlusActive(accountId);
                var needCount = 1;
                if (applyRomanQueueLogicWhenBuilding)
                {
                    needCount++;
                }
                if (isPlusActive)
                {
                    needCount++;
                }
                if (count == needCount)
                {
                    _taskManager.AddOrUpdate<InstantUpgradeTask>(accountId, villageId);
                }
            }
        }
    }
}