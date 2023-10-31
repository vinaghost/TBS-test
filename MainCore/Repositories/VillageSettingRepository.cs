using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Repositories
{
    [RegisterAsTransient]
    public class VillageSettingRepository : IVillageSettingRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public VillageSettingRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public int GetSetting(VillageId villageId, VillageSettingEnums setting)
        {
            using var context = _contextFactory.CreateDbContext();
            var settingEntity = context.VillagesSetting
               .FirstOrDefault(x => x.VillageId == villageId && x.Setting == setting);
            return settingEntity.Value;
        }

        public int GetSetting(VillageId villageId, VillageSettingEnums settingMin, VillageSettingEnums settingMax)
        {
            var settingValueMin = GetSetting(villageId, settingMin);
            var settingValueMax = GetSetting(villageId, settingMax);
            return Random.Shared.Next(settingValueMin, settingValueMax);
        }

        public bool GetBoolSetting(VillageId villageId, VillageSettingEnums setting)
        {
            var settingEntity = GetSetting(villageId, setting);
            //return settingEntity == 0 ? false : true;
            return settingEntity != 0;
        }

        public Dictionary<VillageSettingEnums, int> Get(VillageId villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var settings = context.VillagesSetting.Where(x => x.VillageId == villageId).ToDictionary(x => x.Setting, x => x.Value);
            return settings;
        }

        public void Set(VillageId villageId, Dictionary<VillageSettingEnums, int> settings)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.VillagesSetting.Where(x => x.VillageId == villageId);

            foreach (var setting in settings)
            {
                query
                    .Where(x => x.Setting == setting.Key)
                    .ExecuteUpdate(x => x.SetProperty(x => x.Value, setting.Value));
            }
        }
    }
}