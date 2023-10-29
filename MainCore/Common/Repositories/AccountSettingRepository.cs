using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Common.Repositories
{
    [RegisterAsTransient]
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

        public async Task<int> GetByName(AccountId accountId, AccountSettingEnums setting)
        {
            using var context = _contextFactory.CreateDbContext();
            var settingValue = await Task.Run(() =>
                context.AccountsSetting
                   .Where(x => x.AccountId == accountId && x.Setting == setting)
                   .Select(x => x.Value)
                   .FirstOrDefault());
            return settingValue;
        }

        public async Task<int> GetByName(AccountId accountId, AccountSettingEnums settingMin, AccountSettingEnums settingMax)
        {
            var settingValueMin = await GetByName(accountId, settingMin);
            var settingValueMax = await GetByName(accountId, settingMax);
            return Random.Shared.Next(settingValueMin, settingValueMax);
        }

        public async Task<bool> GetBooleanByName(AccountId accountId, AccountSettingEnums setting)
        {
            var settingEntity = await GetByName(accountId, setting);
            //return settingEntity == 0 ? false : true;
            return settingEntity != 0;
        }

        public async Task FillSetting(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var settings = await Task.Run(() =>
                context.AccountsSetting
                    .Where(x => x.AccountId == accountId)
                    .ToList());
            foreach (var setting in _defaultSettings.Keys)
            {
                var settingEntity = settings.FirstOrDefault(x => x.Setting == setting);
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
            await Task.Run(context.SaveChanges);
        }

        public async Task<Dictionary<AccountSettingEnums, int>> GetDictionary(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var settings = await Task.Run(() =>
                context.AccountsSetting
                    .Where(x => x.AccountId == accountId)
                    .ToDictionary(x => x.Setting, x => x.Value));
            return settings;
        }

        public async Task SetDictionary(AccountId accountId, Dictionary<AccountSettingEnums, int> settings)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.AccountsSetting
                .Where(x => x.AccountId == accountId);

            foreach (var setting in settings)
            {
                await Task.Run(() =>
                    query
                        .Where(x => x.Setting == setting.Key)
                        .ExecuteUpdate(x => x.SetProperty(x => x.Value, setting.Value)));
            }
        }
    }
}