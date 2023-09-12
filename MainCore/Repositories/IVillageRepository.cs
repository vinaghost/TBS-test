using MainCore.Models;

namespace MainCore.Repositories
{
    public interface IVillageRepository
    {
        Task<Village> Get(int villageId);
        Task<List<Village>> GetList(int accountId);
        Task<List<Village>> GetUnloadList(int accountId);
        Task Update(int accountId, List<Village> villages);
    }
}