using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Repositories
{
    public interface IJobRepository
    {
        void Add<T>(VillageId villageId, T content);

        void AddToTop<T>(VillageId villageId, T content);

        int CountBuildingJob(VillageId villageId);

        void Delete(VillageId villageId);

        VillageId Delete(JobId jobId);

        JobDto GetBuildingJob(VillageId villageId);

        JobDto GetInfrastructureBuildingJob(VillageId villageId);

        JobDto GetResourceBuildingJob(VillageId villageId);
        VillageId Move(JobId oldJobId, JobId newJobId);
    }
}