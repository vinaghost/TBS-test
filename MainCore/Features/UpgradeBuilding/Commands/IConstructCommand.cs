using FluentResults;
using MainCore.Common.Models;
using MainCore.Entities;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    public interface IConstructCommand
    {
        Result Execute(AccountId accountId, NormalBuildPlan plan);
    }
}