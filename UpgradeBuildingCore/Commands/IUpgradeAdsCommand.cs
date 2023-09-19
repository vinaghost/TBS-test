using FluentResults;
using MainCore.Models.Plans;

namespace UpgradeBuildingCore.Commands
{
    public interface IUpgradeAdsCommand
    {
        Task<Result> Execute(int accountId, NormalBuildPlan plan);
    }
}