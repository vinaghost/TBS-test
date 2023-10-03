using FluentResults;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    public interface IAddCroplandCommand
    {
        Task<Result> Execute(int villageId);
    }
}