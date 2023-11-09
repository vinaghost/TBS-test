using MainCore.Entities;

namespace MainCore.Repositories
{
    public interface IVillageRepository
    {
        VillageId GetActiveVillages(AccountId accountId);
        List<VillageId> GetHasBuildingJobVillages(AccountId accountId);
        List<VillageId> GetInactiveVillages(AccountId accountId);
        List<VillageId> GetMissingBuildingVillages(AccountId accountId);
        string GetVillageName(VillageId villageId);
    }
}