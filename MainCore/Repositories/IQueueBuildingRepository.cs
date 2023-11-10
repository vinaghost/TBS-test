using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Repositories
{
    public interface IQueueBuildingRepository
    {
        QueueBuilding GetFirst(VillageId villageId);

        void Update(VillageId villageId, List<BuildingDto> dtos);

        void Update(VillageId villageId, List<QueueBuildingDto> dtos);
    }
}