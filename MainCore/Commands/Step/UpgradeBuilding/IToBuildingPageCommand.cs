using FluentResults;
using MainCore.Common.Models;
using MainCore.Entities;

namespace MainCore.Commands.Step.UpgradeBuilding
{
    public interface IToBuildingPageCommand
    {
        Result Execute(AccountId accountId, VillageId villageId, NormalBuildPlan plan);
    }
}