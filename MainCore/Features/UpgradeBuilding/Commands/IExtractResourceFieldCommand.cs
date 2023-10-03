using FluentResults;
using MainCore.Entities;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    public interface IExtractResourceFieldCommand
    {
        Task<Result> Execute(int villageId, Job job);
    }
}