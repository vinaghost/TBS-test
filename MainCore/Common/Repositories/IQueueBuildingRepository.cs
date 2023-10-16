using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IQueueBuildingRepository
    {
        QueueBuilding GetFirst(int villageId);

        List<QueueBuilding> GetList(int villageId);

        void Update(int villageId, List<Building> buildings);

        void Update(int villageId, List<QueueBuilding> queueBuildings);
    }
}