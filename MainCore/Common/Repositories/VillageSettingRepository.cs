using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Common.Repositories
{
    [RegisterAsTransient]
    public class VillageSettingRepository : IVillageSettingRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        private readonly Dictionary<VillageSettingEnums, int> _defaultSettings = new()
        {
            { VillageSettingEnums.UseHeroResourceForBuilding , 0},
            { VillageSettingEnums.ApplyRomanQueueLogicWhenBuilding , 0 },
            { VillageSettingEnums.UseSpecialUpgrade , 0 },
        };

        public VillageSettingRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public int GetSetting(VillageId villageId, VillageSettingEnums setting)
        {
            var settingEntity = _context.VillagesSetting
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

        public void CheckSetting(IDbContextFactory<AppDbContext> contextFactory, VillageId villageId)
        {
            var settings = _context
                .VillagesSetting
                .Where(x => x.VillageId == villageId)
                .ToList();
            foreach (var setting in _defaultSettings.Keys)
            {
                var settingEntity = settings.FirstOrDefault(x => x.Setting == setting);
                if (settingEntity is null)
                {
                    settingEntity = new VillageSetting()
                    {
                        VillageId = villageId,
                        Setting = setting,
                        Value = _defaultSettings[setting],
                    };

                    _context.Add(settingEntity);
                }
            }
            _context.SaveChanges();
        }

        public Dictionary<VillageSettingEnums, int> Get(VillageId villageId)
        {
            var settings = _context.VillagesSetting.Where(x => x.VillageId == villageId).ToDictionary(x => x.Setting, x => x.Value);
            return settings;
        }

        public void Set(VillageId villageId, Dictionary<VillageSettingEnums, int> settings)
        {
            var query = _context.VillagesSetting.Where(x => x.VillageId == villageId);

            foreach (var setting in settings)
            {
                query
                    .Where(x => x.Setting == setting.Key)
                    .ExecuteUpdate(x => x.SetProperty(x => x.Value, setting.Value));
            }
        }
    }
}