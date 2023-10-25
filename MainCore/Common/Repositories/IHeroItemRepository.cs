using FluentResults;
using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IHeroItemRepository
    {
        Result IsEnoughResource(AccountId accountId, long[] requiredResource);

        void Update(AccountId accountId, IEnumerable<HeroItem> items);
    }
}