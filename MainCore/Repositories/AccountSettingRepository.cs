using MainCore.Enums;
using MainCore.Models;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Repositories
{
    public class AccountSettingRepository : IAccountSettingRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        private readonly Dictionary<AccountSettingEnums, int> _defaultSettings = new()
        {
            { AccountSettingEnums.ClickDelayMin , 500},
            { AccountSettingEnums.ClickDelayMax , 900 },
            { AccountSettingEnums.TaskDelayMin , 1000},
            { AccountSettingEnums.TaskDelayMax , 1400 },
            { AccountSettingEnums.IsAutoLoadVillage , 0 },
        };

        public AccountSettingRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<int> GetSetting(int accountId, AccountSettingEnums setting)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var settingEntity = await context.AccountsSetting.FirstOrDefaultAsync(x => x.AccountId == accountId && x.Setting == setting);
            return settingEntity.Value;
        }

        public async Task<int> GetSetting(int accountId, AccountSettingEnums settingMin, AccountSettingEnums settingMax)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var settingEntityMin = await context.AccountsSetting.FirstOrDefaultAsync(x => x.AccountId == accountId && x.Setting == settingMin);
            var settingEntityMax = await context.AccountsSetting.FirstOrDefaultAsync(x => x.AccountId == accountId && x.Setting == settingMax);
            return Random.Shared.Next(settingEntityMin.Value, settingEntityMax.Value);
        }

        public async Task<bool> GetBoolSetting(int accountId, AccountSettingEnums setting)
        {
            var settingEntity = await GetSetting(accountId, setting);
            //return settingEntity == 0 ? false : true;
            return settingEntity != 0;
        }

        public async Task CheckSetting(int accountId, AppDbContext context)
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

                    await context.AddAsync(settingEntity);
                }
            }
            await context.SaveChangesAsync();
        }
    }
}