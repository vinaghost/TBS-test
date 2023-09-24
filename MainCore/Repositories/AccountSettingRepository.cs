using MainCore.Enums;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Repositories
{
    public class AccountSettingRepository : IAccountSettingRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

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
    }
}