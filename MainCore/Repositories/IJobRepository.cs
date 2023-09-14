using MainCore.Models;

namespace MainCore.Repositories
{
    public interface IJobRepository
    {
        Task<Job> Add<T>(int villageId, T content);

        Task<List<Job>> GetList(int villageId);
    }
}