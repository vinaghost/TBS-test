using MainCore.Entities;

namespace MainCore.Repositories
{
    public interface IVillageRepository
    {
        VillageId GetActiveVillageId(AccountId accountId);

        List<VillageId> GetInactiveVillageId(AccountId accountId);
        List<VillageId> GetMissingBuildingVillages(AccountId accountId);
        string GetVillageName(VillageId villageId);
    }
}