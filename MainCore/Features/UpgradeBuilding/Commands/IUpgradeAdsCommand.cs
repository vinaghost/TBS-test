using FluentResults;
using MainCore.Common.Models;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    public interface IUpgradeAdsCommand
    {
        Task<Result> Execute(int accountId, NormalBuildPlan plan);
    }
}