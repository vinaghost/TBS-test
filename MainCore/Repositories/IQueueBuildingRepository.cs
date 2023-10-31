using MainCore.Entities;

namespace MainCore.Repositories
{
    public interface IQueueBuildingRepository
    {
        QueueBuilding GetFirst(VillageId villageId);

        List<QueueBuilding> GetList(VillageId villageId);

        void Update(VillageId villageId, List<Building> buildings);

        void Update(VillageId villageId, List<QueueBuilding> queueBuildings);
    }
}