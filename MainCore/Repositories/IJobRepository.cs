using MainCore.Models;

namespace MainCore.Repositories
{
    public interface IJobRepository
    {
        event Func<int, Job, Task> AddActionCompleted;

        event Func<int, Task> Locked;

        event Func<int, Job, Task> DeleteActionCompleted;

        Task<Job> Add<T>(int villageId, T content);

        Task<Job> AddToTop<T>(int villageId, T content);

        Task Clear(int villageId);

        Task CompleteAdd(int villageId, Job job);

        Task CompleteDelete(int villageId, Job job);

        Task<int> CountBuildingJob(int villageId);

        Task Delete(int jobId);

        Task<Job> Get(int jobId);

        Task<Job> GetFirstJob(int villageId);

        Task<List<Job>> GetList(int villageId);

        Task Lock(int villageId);

        Task Move(int jobOldId, int jobNewId);
    }
}