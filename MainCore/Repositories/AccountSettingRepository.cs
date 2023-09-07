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
            { AccountSettingEnums.ClickDelayMin , 600},
            { AccountSettingEnums.ClickDelayMax , 1000 },
        };

        public AccountSettingRepository(IDbContextFactory<AppDbContext> contextFactory)

        {
            _contextFactory = contextFactory;
        }

        public async Task<int> GetSetting(int accountId, AccountSettingEnums setting)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var settingEntity = await context.AccountsSetting.FindAsync(accountId, setting);
            return settingEntity.Value;
        }

        public async Task<bool> GetBoolSetting(int accountId, AccountSettingEnums setting)
        {
            var settingEntity = await GetSetting(accountId, setting);
            //return settingEntity == 0 ? false : true;
            return settingEntity != 0;
        }

        public async Task CheckSetting(int accountId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var settingEntities = await context.AccountsSetting.Where(x => x.AccountId == accountId).ToListAsync();
            foreach (AccountSettingEnums setting in Enum.GetValues(typeof(AccountSettingEnums)))
            {
                var settingEntity = settingEntities.FirstOrDefault(x => x.Setting == setting);
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