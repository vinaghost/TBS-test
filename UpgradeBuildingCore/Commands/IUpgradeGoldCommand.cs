using FluentResults;
using MainCore.Models.Plans;

namespace UpgradeBuildingCore.Commands
{
    public interface IUpgradeGoldCommand
    {
        Task<Result> Execute(int accountId, NormalBuildPlan plan);
    }
}