using MainCore;
using MainCore.Enums;
using MainCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WPFUI.Repositories
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

        public async Task CheckSetting(int accountId, AppDbContext context)
        {
            var query = context.AccountsSetting.Where(x => x.AccountId == accountId);
            foreach (AccountSettingEnums setting in Enum.GetValues(typeof(AccountSettingEnums)))
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