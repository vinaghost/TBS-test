using MainCore.DTO;

namespace MainCore.Common.Repositories
{
    public interface IJobRepository
    {
        Task Add<T>(int villageId, T content);
        Task AddToTop<T>(int villageId, T content);
        Task Clear(int villageId);
        Task<int> CountBuildingJob(int villageId);
        Task Delete(int jobId);
        Task<List<JobDto>> GetAll(int villageId);
        Task<JobDto> GetById(int jobId);
        Task<JobDto> GetFirstJob(int villageId);
        Task<JobDto> GetInfrastructureBuildingJob(int villageId);
        Task<JobDto> GetResourceBuildingJob(int villageId);
        Task Move(int jobOldId, int jobNewId);
    }
}