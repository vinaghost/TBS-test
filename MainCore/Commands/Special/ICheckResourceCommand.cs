using FluentResults;
using MainCore.Common.Models;
using MainCore.Entities;

namespace MainCore.Commands.Special
{
    public interface ICheckResourceCommand
    {
        long[] Value { get; }

        Task<Result> Execute(AccountId accountId, VillageId villageId, NormalBuildPlan plan);
    }
}