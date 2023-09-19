using FluentResults;
using MainCore.Errors.Storage;
using MainCore.Models;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Repositories
{
    public class StorageRepository : IStorageRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public StorageRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task Update(int villageId, Storage storage)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var storageOnDb = await context.Storages.FirstOrDefaultAsync(x => x.VillageId == villageId);
            if (storageOnDb is null)
            {
                await context.AddAsync(storage);
            }
            else
            {
                storageOnDb.FreeCrop = storage.FreeCrop;
                storageOnDb.Crop = storage.Crop;
                storageOnDb.Iron = storage.Iron;
                storageOnDb.Clay = storage.Clay;
                storageOnDb.Iron = storage.Iron;
                storageOnDb.Warehouse = storage.Warehouse;
                storageOnDb.Granary = storage.Granary;
                context.Update(storageOnDb);
            }

            await context.SaveChangesAsync();
        }

        public async Task<Result> IsEnoughResource(int villageId, long[] requiredResource)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var storage = await context.Storages.FirstOrDefaultAsync(x => x.VillageId == villageId);
            var result = Result.Ok();
            if (storage.Wood < requiredResource[0]) result.WithError(new Resource("wood", storage.Wood, requiredResource[0]));
            if (storage.Clay < requiredResource[1]) result.WithError(new Resource("clay", storage.Clay, requiredResource[1]));
            if (storage.Iron < requiredResource[2]) result.WithError(new Resource("iron", storage.Iron, requiredResource[2]));
            if (storage.Crop < requiredResource[3]) result.WithError(new Resource("crop", storage.Wood, requiredResource[3]));
            if (storage.FreeCrop < requiredResource[4]) result.WithError(new FreeCrop(storage.Wood, requiredResource[4]));

            var max = requiredResource.Max();
            if (storage.Warehouse < max) result.WithError(new WarehouseLimit(storage.Warehouse, max));
            if (storage.Granary < requiredResource[3]) result.WithError(new GranaryLimit(storage.Granary, requiredResource[3]));
            return result;
        }
    }
}