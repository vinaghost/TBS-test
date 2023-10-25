using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IVillageRepository
    {
        Task<VillageId> GetActiveVillageId(AccountId accountId);

        Task<VillageDto> GetById(VillageId villageId);

        Task<List<VillageId>> GetInactiveVillageId(AccountId accountId);

        Task<List<VillageDto>> GetAll(AccountId accountId);

        Task<List<VillageId>> GetUnloadVillageId(AccountId accountId);

        string GetVillageName(VillageId villageId);
    }
}