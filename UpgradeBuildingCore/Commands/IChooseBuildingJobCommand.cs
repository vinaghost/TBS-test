using FluentResults;
using MainCore.Models;

namespace UpgradeBuildingCore.Commands
{
    public interface IChooseBuildingJobCommand
    {
        Job Value { get; }

        Task<Result> Execute(int accountId, int villageId);
    }
}