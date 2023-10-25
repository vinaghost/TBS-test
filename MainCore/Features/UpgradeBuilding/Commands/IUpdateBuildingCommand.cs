using FluentResults;
using MainCore.Entities;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    public interface IUpdateBuildingCommand
    {
        Task<Result> Execute(AccountId accountId, VillageId villageId);
    }
}