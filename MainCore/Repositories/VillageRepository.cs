using MainCore.Models;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Repositories
{
    public class VillageRepository : IVillageRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public VillageRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Village>> Get(int accountId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Villages.Where(x => x.AccountId == accountId).ToListAsync();
        }

        public async Task Update(int accountId, List<Village> villages)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var villagesOnDb = await context.Villages.Where(x => x.AccountId == accountId).ToListAsync();

            var newVillages = villages.Except(villagesOnDb).ToList();
            var oldVillages = villagesOnDb.Except(villages).ToList();
            var updateVillages = villagesOnDb.Where(x => !oldVillages.Contains(x)).ToList();

            await context.AddRangeAsync(newVillages);
            context.RemoveRange(oldVillages);
            foreach (var village in updateVillages)
            {
                var vill = villages.FirstOrDefault(x => x.Id == village.Id);
                if (vill is null) break;

                village.Name = vill.Name;
            }
            context.UpdateRange(updateVillages);

            await context.SaveChangesAsync();
        }
    }
}