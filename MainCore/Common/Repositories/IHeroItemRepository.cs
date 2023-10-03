using FluentResults;
using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IHeroItemRepository
    {
        Task<Result> IsEnoughResource(int accountId, long[] requiredResource);

        Task Update(int accountId, IEnumerable<HeroItem> items);
    }
}