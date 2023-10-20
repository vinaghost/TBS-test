using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Common.Repositories
{
    [RegisterAsSingleton]
    public class VillageSettingRepository : IVillageSettingRepository
    {
        private readonly AppDbContext _context;

        private readonly Dictionary<VillageSettingEnums, int> _defaultSettings = new()
        {
            { VillageSettingEnums.UseHeroResourceForBuilding , 0},
            { VillageSettingEnums.ApplyRomanQueueLogicWhenBuilding , 0 },
            { VillageSettingEnums.UseSpecialUpgrade , 0 },
        };

        public VillageSettingRepository(AppDbContext context)
        {
            _context = context;
        }

        private int GetSetting(AppDbContext context, int villageId, VillageSettingEnums setting)
        {
            var settingEntity = _context.VillagesSetting
               .FirstOrDefault(x => x.VillageId == villageId && x.Setting == setting);
            return settingEntity.Value;
        }

        public int GetSetting(int villageId, VillageSettingEnums setting)
        {
           
            return GetSetting(context, villageId, setting);
        }

        public int GetSetting(int villageId, VillageSettingEnums settingMin, VillageSettingEnums settingMax)
        {
           
            var settingValueMin = GetSetting(context, villageId, settingMin);
            var settingValueMax = GetSetting(context, villageId, settingMax);
            return Random.Shared.Next(settingValueMin, settingValueMax);
        }

        public bool GetBoolSetting(int villageId, VillageSettingEnums setting)
        {
            var settingEntity = GetSetting(villageId, setting);
            //return settingEntity == 0 ? false : true;
            return settingEntity != 0;
        }

        public void CheckSetting(AppDbContext context, int villageId)
        {
            var query = _context.VillagesSetting.Where(x => x.VillageId == villageId);
            foreach (var setting in _defaultSettings.Keys)
            {
                var settingEntity = query.FirstOrDefault(x => x.Setting == setting);
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

        public Dictionary<VillageSettingEnums, int> Get(int villageId)
        {
           
            var settings = _context.VillagesSetting.Where(x => x.VillageId == villageId).ToDictionary(x => x.Setting, x => x.Value);
            return settings;
        }

        public void Set(int villageId, Dictionary<VillageSettingEnums, int> settings)
        {
           
            var query = _context.VillagesSetting.Where(x => x.VillageId == villageId);

            foreach (var setting in settings)
            {
                query.Where(x => x.Setting == setting.Key).ExecuteUpdate(x => x.SetProperty(x => x.Value, setting.Value));
            }
        }
    }
}