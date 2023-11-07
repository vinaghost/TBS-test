using FluentResults;
using MainCore.Entities;

namespace MainCore.Repositories
{
    public interface IHeroItemRepository
    {
        Result IsEnoughResource(AccountId accountId, long[] requiredResource);
    }
}