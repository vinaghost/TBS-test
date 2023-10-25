using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IJobRepository
    {
        Task Add<T>(VillageId villageId, T content);

        Task AddToTop<T>(VillageId villageId, T content);

        Task Clear(VillageId villageId);

        Task<int> CountBuildingJob(VillageId villageId);

        Task DeleteById(JobId jobId);

        Task<List<JobDto>> GetAll(VillageId villageId);

        Task<JobDto> GetById(JobId jobId);

        Task<JobDto> GetFirstJob(VillageId villageId);

        Task<JobDto> GetInfrastructureBuildingJob(VillageId villageId);

        Task<JobDto> GetResourceBuildingJob(VillageId villageId);

        Task Move(JobId jobOldId, JobId jobNewId);
    }
}