using FluentResults;
using MainCore.Models.Plans;

namespace UpgradeBuildingCore.Commands
{
    public interface IGoToBuildingPageCommand
    {
        Task<Result> Execute(int accountId, int villageId, NormalBuildPlan plan);
    }
}