using FluentResults;
using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IStorageRepository
    {
        Result IsEnoughResource(int villageId, long[] requiredResource);

        long[] GetMissingResource(int villageId, long[] requiredResource);

        void Update(int villageId, Storage storage);
    }
}