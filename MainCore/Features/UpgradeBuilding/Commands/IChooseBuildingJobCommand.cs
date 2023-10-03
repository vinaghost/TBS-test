using FluentResults;
using MainCore.Entities;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    public interface IChooseBuildingJobCommand
    {
        Job Value { get; }

        Task<Result> Execute(int accountId, int villageId);
    }
}