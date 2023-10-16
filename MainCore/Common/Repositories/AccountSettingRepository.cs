using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Common.Repositories
{
    [RegisterAsSingleton]
    public class AccountSettingRepository : IAccountSettingRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        private readonly Dictionary<AccountSettingEnums, int> _defaultSettings = new()
        {
            { AccountSettingEnums.ClickDelayMin , 500}, // ms
            { AccountSettingEnums.ClickDelayMax , 900 }, // ms
            { AccountSettingEnums.TaskDelayMin , 1000}, // ms
            { AccountSettingEnums.TaskDelayMax , 1400 }, // ms
            { AccountSettingEnums.IsAutoLoadVillage , 0 },
            { AccountSettingEnums.FarmIntervalMin , 600 }, // s
            { AccountSettingEnums.FarmIntervalMax , 900 }, // s
            { AccountSettingEnums.UseStartAllButton , 0 },
        };

        public AccountSettingRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        private int GetSetting(AppDbContext context, int accountId, AccountSettingEnums setting)
        {
            var settingEntity = context.AccountsSetting
               .FirstOrDefault(x => x.AccountId == accountId && x.Setting == setting);
            return settingEntity.Value;
        }

        public int GetSetting(int accountId, AccountSettingEnums setting)
        {
            using var context = _contextFactory.CreateDbContext();
            return GetSetting(context, accountId, setting);
        }

        public int GetSetting(int accountId, AccountSettingEnums settingMin, AccountSettingEnums settingMax)
        {
            using var context = _contextFactory.CreateDbContext();
            var settingValueMin = GetSetting(context, accountId, settingMin);
            var settingValueMax = GetSetting(context, accountId, settingMax);
            return Random.Shared.Next(settingValueMin, settingValueMax);
        }

        public bool GetBoolSetting(int accountId, AccountSettingEnums setting)
        {
            var settingEntity = GetSetting(accountId, setting);
            //return settingEntity == 0 ? false : true;
            return settingEntity != 0;
        }

        public void CheckSetting(AppDbContext context, int accountId)
        {
            var query = context.AccountsSetting.Where(x => x.AccountId == accountId);
            foreach (var setting in _defaultSettings.Keys)
            {
                var settingEntity = query.FirstOrDefault(x => x.Setting == setting);
                if (settingEntity is null)
                {
                    settingEntity = new AccountSetting()
                    {
                        AccountId = accountId,
                        Setting = setting,
                        Value = _defaultSettings[setting],
                    };

                    context.Add(settingEntity);
                }
            }
            context.SaveChanges();
        }

        public Dictionary<AccountSettingEnums, int> Get(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var settings = context.AccountsSetting
                .Where(x => x.AccountId == accountId)
                .ToDictionary(x => x.Setting, x => x.Value);
            return settings;
        }

        public void Set(int accountId, Dictionary<AccountSettingEnums, int> settings)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.AccountsSetting
                .Where(x => x.AccountId == accountId);

            foreach (var setting in settings)
            {
                query
                    .Where(x => x.Setting == setting.Key)
                    .ExecuteUpdate(x => x.SetProperty(x => x.Value, setting.Value));
            }
        }
    }
}