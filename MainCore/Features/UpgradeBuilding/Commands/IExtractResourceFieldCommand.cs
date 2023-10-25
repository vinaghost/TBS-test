using FluentResults;
using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    public interface IExtractResourceFieldCommand
    {
        Task<Result> Execute(VillageId villageId, JobDto job);
    }
}