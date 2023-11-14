using FluentResults;
using MainCore.Common.Models;
using MainCore.Entities;

namespace MainCore.Commands.Special
{
    public interface IGetRequiredResourceCommand
    {
        long[] Value { get; }

        Result Execute(AccountId accountId, VillageId villageId, NormalBuildPlan plan);
    }
}