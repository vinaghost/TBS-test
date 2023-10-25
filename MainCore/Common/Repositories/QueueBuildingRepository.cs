using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;

namespace MainCore.Common.Repositories
{
    [RegisterAsTransient]
    public class QueueBuildingRepository : IQueueBuildingRepository
    {
        private readonly AppDbContext _context;
        private readonly IVillageSettingRepository _villageSettingRepository;
        private readonly IAccountInfoRepository _accountInfoRepository;
        private readonly ITaskManager _taskManager;

        public QueueBuildingRepository(AppDbContext context, IVillageSettingRepository villageSettingRepository, IAccountInfoRepository accountInfoRepository, ITaskManager taskManager)
        {
            _context = context;
            _villageSettingRepository = villageSettingRepository;
            _accountInfoRepository = accountInfoRepository;
            _taskManager = taskManager;
        }

        public QueueBuilding GetFirst(VillageId villageId)
        {
            var queueBuildings = _context.QueueBuildings
                .Where(x => x.VillageId == villageId && x.Type != BuildingEnums.Site)
                .OrderBy(x => x.Position);
            var queueBuilding = queueBuildings.FirstOrDefault();
            return queueBuilding;
        }

        public List<QueueBuilding> GetList(VillageId villageId)
        {
            var queueBuildings = _context.QueueBuildings
                .Where(x => x.VillageId == villageId && x.Type != BuildingEnums.Site)
                .OrderBy(x => x.Position);
            return queueBuildings.ToList();
        }

        public void Update(VillageId villageId, List<Building> buildings)
        {
            var queueBuildings = _context.QueueBuildings
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
                _context.UpdateRange(list);
            }
            else if (buildings.Count == 2)
            {
                foreach (var building in buildings)
                {
                    var queueBuilding = queueBuildings.FirstOrDefault(x => x.Type == building.Type);
                    queueBuilding.Location = building.Location;
                    _context.Update(queueBuilding);
                }
            }
            _context.SaveChanges();
        }

        public void Update(VillageId villageId, List<QueueBuilding> queueBuildings)
        {
            var dbQueueBuildings = _context.QueueBuildings.Where(x => x.VillageId == villageId);

            for (var i = 0; i < queueBuildings.Count; i++)
            {
                var queueBuilding = queueBuildings[i];
                var dbQueueBuilding = dbQueueBuildings.FirstOrDefault(x => x.Position == i);
                if (dbQueueBuilding is null)
                {
                    _context.Add(queueBuilding);
                }
                else
                {
                    dbQueueBuilding.Type = queueBuilding.Type;
                    dbQueueBuilding.Level = queueBuilding.Level;
                    dbQueueBuilding.CompleteTime = queueBuilding.CompleteTime;
                    _context.Update(dbQueueBuilding);
                }
            }
            _context.SaveChanges();
        }

        //private Task TriggerInstantUpgrade(VillageId villageId)
        //{
        //    var instantUpgrade = _villageSettingRepository.GetBoolSetting(villageId, VillageSettingEnums.InstantUpgrade);
        //    if (instantUpgrade)
        //    {
        //        var applyRomanQueueLogicWhenBuilding = _villageSettingRepository.GetBoolSetting(villageId, VillageSettingEnums.ApplyRomanQueueLogicWhenBuilding);
        //        AccountId accountId, count;
        //        using (var context = _contextFactory.CreateDbContext())
        //        {
        //            var village = _context.Villages.Find(villageId);
        //            accountId = village.VillageId;
        //            count = _context.QueueBuildings.Where(x => x.VillageId == villageId).Count();
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