using FluentResults;

namespace UpgradeBuildingCore.Commands
{
    public interface IAddCroplandCommand
    {
        Task<Result> Execute(int villageId);
    }
}