﻿using MainCore.Models;
using MainCore.Models.Plans;

namespace MainCore.Repositories
{
    public interface IBuildingRepository
    {
        event Func<int, Task> BuildingUpdated;

        Task<int> CountQueueBuilding(int villageId);
        Task<Building> Get(int buildingId);
        Task<Building> GetBasedOnLocation(int villageId, int location);
        Task<Building> GetCropland(int villageId);
        Task<List<Building>> GetList(int villageId);
        Task<NormalBuildPlan> GetNormalBuildPlan(int villageId, ResourceBuildPlan plan);
        Task Update(int villageId, List<Building> buildings);
    }
}