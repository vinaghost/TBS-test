using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IQueueBuildingRepository
    {
        event Func<int, Task> QueueBuildingUpdated;

        Task<QueueBuilding> GetFirst(int villageId);
        Task<List<QueueBuilding>> GetList(int villageId);
        Task Update(int villageId, List<QueueBuilding> queueBuildings);
        Task Update(int villageId, List<Building> buildings);
    }
}