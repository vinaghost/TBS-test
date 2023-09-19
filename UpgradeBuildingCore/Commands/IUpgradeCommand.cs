using FluentResults;
using MainCore.Models.Plans;

namespace UpgradeBuildingCore.Commands
{
    public interface IUpgradeCommand
    {
        Task<Result> Execute(int accountId, NormalBuildPlan plan);
    }
}