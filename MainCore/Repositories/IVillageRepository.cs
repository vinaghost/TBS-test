using MainCore.Models;

namespace MainCore.Repositories
{
    public interface IVillageRepository
    {
        Task<List<Village>> Get(int accountId);
        Task Update(int accountId, List<Village> villages);
    }
}