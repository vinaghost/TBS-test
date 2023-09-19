using FluentResults;
using MainCore.Models.Plans;

namespace UpgradeBuildingCore.Commands
{
    public interface ICheckResourceCommand
    {
        Task<Result> Execute(int accountId, int villageId, NormalBuildPlan plan);
    }
}