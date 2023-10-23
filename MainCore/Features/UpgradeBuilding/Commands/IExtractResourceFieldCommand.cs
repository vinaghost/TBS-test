using FluentResults;
using MainCore.DTO;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    public interface IExtractResourceFieldCommand
    {
        Task<Result> Execute(int villageId, JobDto job);
    }
}