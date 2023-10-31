using MainCore.Common.Models;
using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Repositories
{
    public interface IBuildingRepository
    {
        int CountQueueBuilding(VillageId villageId);

        int CountResourceQueueBuilding(VillageId villageId);
        BuildingDto GetBuilding(VillageId villageId, int location);
        Building GetCropland(VillageId villageId);

        NormalBuildPlan GetNormalBuildPlan(VillageId villageId, ResourceBuildPlan plan);
        bool IsEmptySite(VillageId villageId, int location);
        bool IsJobComplete(VillageId villageId, JobDto job);
        bool IsRallyPointExists(VillageId villageId);
    }
}