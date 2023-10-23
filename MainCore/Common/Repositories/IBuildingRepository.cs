using FluentResults;
using MainCore.Common.Enums;
using MainCore.Common.Models;
using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IBuildingRepository
    {
        List<BuildingEnums> AvailableBuildings { get; }

        Result CheckRequirements(int villageId, NormalBuildPlan plan);

        int CountQueueBuilding(int villageId);

        int CountResourceQueueBuilding(int villageId);

        Building GetBuilding(int buildingId);

        Building GetBuilding(int villageId, int location);

        List<BuildingItemDto> GetBuildingItems(int villageId);

        List<Building> GetBuildingList(int villageId);

        Building GetCropland(int villageId);

        NormalBuildPlan GetNormalBuildPlan(int villageId, ResourceBuildPlan plan);

        bool HasRallyPoint(int villageId);

        bool IsJobValid(int villageId, JobDto job);

        void Update(int villageId, List<Building> buildings);

        void Validate(int villageId, NormalBuildPlan plan);
    }
}