using FluentResults;
using MainCore.Common.Models;
using MainCore.Entities;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    public interface ICheckResourceCommand
    {
        long[] Value { get; }

        Task<Result> Execute(AccountId accountId, VillageId villageId, NormalBuildPlan plan);
    }
}