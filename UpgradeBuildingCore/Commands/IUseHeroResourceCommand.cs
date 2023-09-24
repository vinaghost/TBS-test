using FluentResults;

namespace UpgradeBuildingCore.Commands
{
    public interface IUseHeroResourceCommand
    {
        Task<Result> Execute(int accountId, long[] requiredResource);
    }
}