using FluentResults;
using MainCore.Models;

namespace MainCore.Repositories
{
    public interface IStorageRepository
    {
        Task<Result> IsEnoughResource(int villageId, long[] requiredResource);
        Task<long[]> GetMissingResource(int villageId, long[] requiredResource);
        Task Update(int villageId, Storage storage);
    }
}