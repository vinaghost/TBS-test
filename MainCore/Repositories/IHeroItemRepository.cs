using FluentResults;
using MainCore.Models;

namespace MainCore.Repositories
{
    public interface IHeroItemRepository
    {
        Task<Result> IsEnoughResource(int accountId, long[] requiredResource);

        Task Update(int accountId, IEnumerable<HeroItem> items);
    }
}