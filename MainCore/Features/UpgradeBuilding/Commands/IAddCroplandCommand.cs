using FluentResults;
using MainCore.Entities;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    public interface IAddCroplandCommand
    {
        Task<Result> Execute(VillageId villageId);
    }
}