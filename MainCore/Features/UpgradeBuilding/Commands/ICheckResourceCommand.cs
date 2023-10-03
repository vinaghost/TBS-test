using FluentResults;
using MainCore.Common.Models;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    public interface ICheckResourceCommand
    {
        long[] Value { get; }

        Task<Result> Execute(int accountId, int villageId, NormalBuildPlan plan);
    }
}