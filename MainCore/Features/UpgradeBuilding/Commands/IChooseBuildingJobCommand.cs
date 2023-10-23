using FluentResults;
using MainCore.DTO;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    public interface IChooseBuildingJobCommand
    {
        JobDto Value { get; }

        Task<Result> Execute(int accountId, int villageId);
    }
}