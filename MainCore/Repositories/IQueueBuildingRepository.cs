using MainCore.Entities;

namespace MainCore.Repositories
{
    public interface IQueueBuildingRepository
    {
        QueueBuilding GetFirst(VillageId villageId);
    }
}