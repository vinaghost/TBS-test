using FluentResults;
using MainCore.Common.Enums;
using MainCore.Common.Errors.Storage;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Common.Repositories
{
    [RegisterAsSingleton]
    public class HeroItemRepository : IHeroItemRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public HeroItemRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Result> IsEnoughResource(int accountId, long[] requiredResource)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var items = context.HeroItems.Where(x => x.AccountId == accountId);

            var result = Result.Ok();
            var wood = await items.FirstOrDefaultAsync(x => x.Type == HeroItemEnums.Wood);
            var woodAmount = wood?.Amount ?? 0;
            if (woodAmount < requiredResource[0]) result.WithError(new Resource("wood", woodAmount, requiredResource[0]));
            var clay = await items.FirstOrDefaultAsync(x => x.Type == HeroItemEnums.Clay);
            var clayAmount = clay?.Amount ?? 0;
            if (clayAmount < requiredResource[1]) result.WithError(new Resource("clay", clayAmount, requiredResource[1]));
            var iron = await items.FirstOrDefaultAsync(x => x.Type == HeroItemEnums.Iron);
            var ironAmount = iron?.Amount ?? 0;
            if (ironAmount < requiredResource[2]) result.WithError(new Resource("iron", ironAmount, requiredResource[2]));
            var crop = await items.FirstOrDefaultAsync(x => x.Type == HeroItemEnums.Crop);
            var cropAmount = crop?.Amount ?? 0;
            if (cropAmount < requiredResource[3]) result.WithError(new Resource("crop", cropAmount, requiredResource[3]));
            return result;
        }

        public async Task Update(int accountId, IEnumerable<HeroItem> items)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var dbItems = await context.HeroItems.Where(x => x.AccountId == accountId).ToListAsync();
            foreach (var item in items)
            {
                var dbItem = dbItems.FirstOrDefault(x => x.Type == item.Type);
                if (dbItem is null)
                {
                    await context.AddAsync(item);
                }
                else
                {
                    dbItem.Type = item.Type;
                    dbItem.Amount = item.Amount;
                    context.Update(dbItem);
                }
            }
            await context.SaveChangesAsync();
        }
    }
}