using FluentResults;
using MainCore.Common.Errors.Storage;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;

namespace MainCore.Common.Repositories
{
    [RegisterAsTransient]
    public class StorageRepository : IStorageRepository
    {
        private readonly AppDbContext _context;

        public StorageRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Update(VillageId villageId, Storage storage)
        {
            var storageOnDb = _context.Storages.FirstOrDefault(x => x.VillageId == villageId);
            if (storageOnDb is null)
            {
                _context.Add(storage);
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
                _context.Update(storageOnDb);
            }

            _context.SaveChanges();
        }

        public Result IsEnoughResource(VillageId villageId, long[] requiredResource)
        {
            var storage = _context.Storages.FirstOrDefault(x => x.VillageId == villageId);
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

        public long[] GetMissingResource(VillageId villageId, long[] requiredResource)
        {
            var storage = _context.Storages.FirstOrDefault(x => x.VillageId == villageId);

            var resource = new long[4];
            if (storage.Wood < requiredResource[0]) resource[0] = requiredResource[0] - storage.Wood;
            if (storage.Clay < requiredResource[1]) resource[1] = requiredResource[1] - storage.Clay;
            if (storage.Iron < requiredResource[2]) resource[2] = requiredResource[2] - storage.Iron;
            if (storage.Crop < requiredResource[3]) resource[3] = requiredResource[3] - storage.Crop;
            return resource;
        }
    }
}