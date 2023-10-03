using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IVillageRepository
    {
        event Func<int, Task> VillageListChanged;

        Task<Village> Get(int villageId);

        Task<Village> GetActive(int accountId);

        Task<List<Village>> GetInactive(int accountId);

        Task<List<Village>> GetList(int accountId);

        Task<List<Village>> GetUnloadList(int accountId);

        Task<List<Village>> Update(int accountId, List<Village> villages);
    }
}