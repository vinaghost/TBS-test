using FluentResults;
using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Commands.Special
{
    public interface IChooseBuildingJobCommand
    {
        JobDto Value { get; }

        Task<Result> Execute(AccountId accountId, VillageId villageId);
    }
}