using FluentResults;

namespace UpgradeBuildingCore.Commands
{
    public interface IUpdateBuildingCommand
    {
        Task<Result> Execute(int accountId, int villageId);
    }
}