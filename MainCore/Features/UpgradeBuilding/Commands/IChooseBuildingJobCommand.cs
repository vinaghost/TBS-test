using FluentResults;
using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    public interface IChooseBuildingJobCommand
    {
        JobDto Value { get; }

        Task<Result> Execute(AccountId accountId, VillageId villageId);
    }
}