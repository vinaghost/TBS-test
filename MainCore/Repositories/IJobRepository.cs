using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Repositories
{
    public interface IJobRepository
    {
        Task AddToTop<T>(VillageId villageId, T content);

        int CountBuildingJob(VillageId villageId);

        Task DeleteById(JobId jobId);

        JobDto GetBuildingJob(VillageId villageId);

        JobDto GetInfrastructureBuildingJob(VillageId villageId);

        JobDto GetResourceBuildingJob(VillageId villageId);
    }
}