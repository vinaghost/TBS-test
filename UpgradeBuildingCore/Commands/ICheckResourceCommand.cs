using FluentResults;
using MainCore.Models.Plans;

namespace UpgradeBuildingCore.Commands
{
    public interface ICheckResourceCommand
    {
        long[] Value { get; }

        Task<Result> Execute(int accountId, int villageId, NormalBuildPlan plan);
    }
}