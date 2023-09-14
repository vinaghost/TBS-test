using MainCore.Models;

namespace MainCore.Repositories
{
    public interface IJobRepository
    {
        Task<Job> Add<T>(int villageId, T content);
        Task Clear(int villageId);
        Task Delete(int jobId);
        Task<Job> Get(int jobId);
        Task<List<Job>> GetList(int villageId);
        Task Move(int jobOldId, int jobNewId);
    }
}