using FluentResults;
using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IHeroItemRepository
    {
        Result IsEnoughResource(int accountId, long[] requiredResource);

        void Update(int accountId, IEnumerable<HeroItem> items);
    }
}