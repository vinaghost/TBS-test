using FluentResults;
using MainCore.Common.Models;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    public interface IGoToBuildingPageCommand
    {
        Task<Result> Execute(int accountId, int villageId, NormalBuildPlan plan);
    }
}