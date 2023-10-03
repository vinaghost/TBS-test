using FluentResults;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    public interface IUseHeroResourceCommand
    {
        Task<Result> Execute(int accountId, long[] requiredResource);
    }
}