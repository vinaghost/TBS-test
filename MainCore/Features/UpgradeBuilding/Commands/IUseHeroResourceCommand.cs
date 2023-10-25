using FluentResults;
using MainCore.Entities;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    public interface IUseHeroResourceCommand
    {
        Task<Result> Execute(AccountId accountId, long[] requiredResource);
    }
}