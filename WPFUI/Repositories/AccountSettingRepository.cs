using MainCore.Common.Enums;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WPFUI.Repositories
{
    public class AccountSettingRepository : IAccountSettingRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public AccountSettingRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Dictionary<AccountSettingEnums, int>> Get(int accountId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var settings = await context.AccountsSetting.Where(x => x.AccountId == accountId).ToDictionaryAsync(x => x.Setting, x => x.Value);
            return settings;
        }

        public async Task Set(int accountId, Dictionary<AccountSettingEnums, int> settings)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.AccountsSetting.Where(x => x.AccountId == accountId);

            foreach (var setting in settings)
            {
                await query.Where(x => x.Setting == setting.Key).ExecuteUpdateAsync(x => x.SetProperty(x => x.Value, setting.Value));
            }
        }
    }
}