using MainCore;
using MainCore.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WPFUI.Repositories
{
    public class VillageSettingRepository : IVillageSettingRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public VillageSettingRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
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