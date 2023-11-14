using FluentResults;
using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Commands.Step.UpgradeBuilding
{
    public interface IExtractResourceFieldCommand
    {
        Task<Result> Execute(VillageId villageId, JobDto job);
    }
}