using FluentResults;
using MainCore.Models.Plans;

namespace UpgradeBuildingCore.Commands
{
    public interface IConstructCommand
    {
        Task<Result> Execute(int accountId, NormalBuildPlan plan);
    }
}