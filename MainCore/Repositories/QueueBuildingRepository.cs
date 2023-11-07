using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Repositories
{
    [RegisterAsTransient]
    public class QueueBuildingRepository : IQueueBuildingRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public QueueBuildingRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public QueueBuilding GetFirst(VillageId villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var queueBuilding = context.QueueBuildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Type != BuildingEnums.Site)
                .OrderBy(x => x.Position)
                .FirstOrDefault();
            return queueBuilding;
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
        //            var village = context.Villages.Find(villageId);
        //            accountId = village.VillageId;
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