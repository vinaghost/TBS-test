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

        public async Task<int> GetSetting(int villageId, VillageSettingEnums setting)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var settingEntity = await context.VillagesSetting.FirstOrDefaultAsync(x => x.VillageId == villageId && x.Setting == setting);
            return settingEntity.Value;
        }

        public async Task<int> GetSetting(int villageId, VillageSettingEnums settingMin, VillageSettingEnums settingMax)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var settingEntityMin = await context.VillagesSetting.FirstOrDefaultAsync(x => x.VillageId == villageId && x.Setting == settingMin);
            var settingEntityMax = await context.VillagesSetting.FirstOrDefaultAsync(x => x.VillageId == villageId && x.Setting == settingMax);
            return Random.Shared.Next(settingEntityMin.Value, settingEntityMax.Value);
        }

        public async Task<bool> GetBoolSetting(int villageId, VillageSettingEnums setting)
        {
            var settingEntity = await GetSetting(villageId, setting);
            //return settingEntity == 0 ? false : true;
            return settingEntity != 0;
        }

        public async Task CheckSetting(int villageId, AppDbContext context)
        {
            var query = context.VillagesSetting.Where(x => x.VillageId == villageId);
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

                    await context.AddAsync(settingEntity);
                }
            }
            await context.SaveChangesAsync();
        }

        public async Task<Dictionary<VillageSettingEnums, int>> Get(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var settings = await context.VillagesSetting.Where(x => x.VillageId == villageId).ToDictionaryAsync(x => x.Setting, x => x.Value);
            return settings;
        }

        public async Task Set(int villageId, Dictionary<VillageSettingEnums, int> settings)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.VillagesSetting.Where(x => x.VillageId == villageId);

            foreach (var setting in settings)
            {
                await query.Where(x => x.Setting == setting.Key).ExecuteUpdateAsync(x => x.SetProperty(x => x.Value, setting.Value));
            }
        }
    }
}