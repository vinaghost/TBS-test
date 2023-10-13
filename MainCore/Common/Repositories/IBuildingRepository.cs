using FluentResults;
using MainCore.Common.Enums;
using MainCore.Common.Models;
using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IBuildingRepository
    {
        event Func<int, Task> BuildingUpdated;

        Task<Result> CheckRequirements(int villageId, NormalBuildPlan plan);
        Task<int> CountInfrastructureQueueBuilding(int villageId);
        Task<int> CountQueueBuilding(int villageId);
        Task<int> CountResourceQueueBuilding(int villageId);
        Task<Building> Get(int buildingId);
        List<BuildingEnums> GetAvailableBuildings();
        Task<Building> GetBasedOnLocation(int villageId, int location);
        Task<List<BuildingItemDto>> GetBuildingItems(int villageId);
        Task<Building> GetCropland(int villageId);
        Task<List<Building>> GetList(int villageId);
        Task<NormalBuildPlan> GetNormalBuildPlan(int villageId, ResourceBuildPlan plan);
        Task<bool> HasRallyPoint(int villageId);
        Task<bool> IsValid(int villageId, Job job);
        Task Update(int villageId, List<Building> buildings);
        Task Validate(int villageId, NormalBuildPlan plan);
    }
}