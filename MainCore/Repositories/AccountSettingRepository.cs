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
            var settingEntity = await context.AccountsSetting.FindAsync(accountId, setting);
            return settingEntity.Value;
        }

        public async Task<bool> GetBoolSetting(int accountId, AccountSettingEnums setting)
        {
            var settingEntity = await GetSetting(accountId, setting);
            //return settingEntity == 0 ? false : true;
            return settingEntity != 0;
        }
    }
}