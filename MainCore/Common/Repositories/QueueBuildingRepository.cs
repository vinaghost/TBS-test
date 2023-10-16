using MainCore.Common.Enums;
using MainCore.Entities;
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

        public QueueBuildingRepository(IDbContextFactory<AppDbContext> contextFactory, IVillageSettingRepository villageSettingRepository, IAccountInfoRepository accountInfoRepository, ITaskManager taskManager)
        {
            _contextFactory = contextFactory;
            _villageSettingRepository = villageSettingRepository;
            _accountInfoRepository = accountInfoRepository;
            _taskManager = taskManager;
        }

        public QueueBuilding GetFirst(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var queueBuildings = context.QueueBuildings
                .Where(x => x.VillageId == villageId && x.Type != BuildingEnums.Site)
                .OrderBy(x => x.Position);
            var queueBuilding = queueBuildings.FirstOrDefault();
            return queueBuilding;
        }

        public List<QueueBuilding> GetList(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var queueBuildings = context.QueueBuildings
                .Where(x => x.VillageId == villageId && x.Type != BuildingEnums.Site)
                .OrderBy(x => x.Position);
            return queueBuildings.ToList();
        }

        public void Update(int villageId, List<Building> buildings)
        {
            using var context = _contextFactory.CreateDbContext();

            var queueBuildings = context.QueueBuildings
                .Where(x => x.VillageId == villageId && x.Type != BuildingEnums.Site);

            if (buildings.Count == 1)
            {
                var building = buildings[0];
                queueBuildings = queueBuildings.Where(x => x.Type == building.Type);
                var list = queueBuildings.ToList();
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
                    var queueBuilding = queueBuildings.FirstOrDefault(x => x.Type == building.Type);
                    queueBuilding.Location = building.Location;
                    context.Update(queueBuilding);
                }
            }
            context.SaveChanges();
        }

        public void Update(int villageId, List<QueueBuilding> queueBuildings)
        {
            using var context = _contextFactory.CreateDbContext();

            var dbQueueBuildings = context.QueueBuildings.Where(x => x.VillageId == villageId);

            for (var i = 0; i < queueBuildings.Count; i++)
            {
                var queueBuilding = queueBuildings[i];
                var dbQueueBuilding = dbQueueBuildings.FirstOrDefault(x => x.Position == i);
                if (dbQueueBuilding is null)
                {
                    context.Add(queueBuilding);
                }
                else
                {
                    dbQueueBuilding.Type = queueBuilding.Type;
                    dbQueueBuilding.Level = queueBuilding.Level;
                    dbQueueBuilding.CompleteTime = queueBuilding.CompleteTime;
                    context.Update(dbQueueBuilding);
                }
            }
            context.SaveChanges();
        }

        //private Task TriggerInstantUpgrade(int villageId)
        //{
        //    var instantUpgrade = _villageSettingRepository.GetBoolSetting(villageId, VillageSettingEnums.InstantUpgrade);
        //    if (instantUpgrade)
        //    {
        //        var applyRomanQueueLogicWhenBuilding = _villageSettingRepository.GetBoolSetting(villageId, VillageSettingEnums.ApplyRomanQueueLogicWhenBuilding);
        //        int accountId, count;
        //        using (var context = _contextFactory.CreateDbContext())
        //        {
        //            var village = context.Villages.Find(villageId);
        //            accountId = village.AccountId;
        //            count = context.QueueBuildings.Where(x => x.VillageId == villageId).Count();
        //        }

        //        var isPlusActive = _accountInfoRepository.IsPlusActive(accountId);
        //        var needCount = 1;
        //        if (applyRomanQueueLogicWhenBuilding)
        //        {
        //            needCount++;
        //        }
        //        if (isPlusActive)
        //        {
        //            needCount++;
        //        }
        //        if (count == needCount)
        //        {
        //            _taskManager.AddOrUpdate<InstantUpgradeTask>(accountId, villageId);
        //        }
        //    }
        //}
    }
}