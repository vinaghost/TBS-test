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

        Result CheckRequirements(VillageId villageId, NormalBuildPlan plan);

        int CountQueueBuilding(VillageId villageId);

        int CountResourceQueueBuilding(VillageId villageId);

        Building GetBuilding(int buildingId);

        Building GetBuilding(VillageId villageId, int location);

        List<BuildingItemDto> GetBuildingItems(VillageId villageId);

        List<Building> GetBuildingList(VillageId villageId);

        Building GetCropland(VillageId villageId);

        NormalBuildPlan GetNormalBuildPlan(VillageId villageId, ResourceBuildPlan plan);

        bool HasRallyPoint(VillageId villageId);

        bool IsJobValid(VillageId villageId, JobDto job);

        void Update(VillageId villageId, List<Building> buildings);

        void Validate(VillageId villageId, NormalBuildPlan plan);
    }
}