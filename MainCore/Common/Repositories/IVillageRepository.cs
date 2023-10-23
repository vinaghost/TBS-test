using MainCore.DTO;

namespace MainCore.Common.Repositories
{
    public interface IVillageRepository
    {
        Task<int> GetActiveVillageId(int accountId);

        Task<VillageDto> GetById(int villageId);

        Task<List<int>> GetInactiveVillageId(int accountId);

        Task<List<VillageDto>> GetAll(int accountId);

        Task<List<int>> GetUnloadVillageId(int accountId);
        string GetVillageName(int villageId);
    }
}