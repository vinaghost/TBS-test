using FluentResults;
using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IStorageRepository
    {
        Result IsEnoughResource(VillageId villageId, long[] requiredResource);

        long[] GetMissingResource(VillageId villageId, long[] requiredResource);

        void Update(VillageId villageId, Storage storage);
    }
}