using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IJobRepository
    {
        JobDto Add<T>(int villageId, T content);

        Job AddToTop<T>(int villageId, T content);

        void Clear(int villageId);

        int CountBuildingJob(int villageId);

        void Delete(int jobId);

        Job Get(int jobId);

        Job GetFirstJob(int villageId);

        Job GetInfrastructureBuildingJob(int villageId);

        List<JobDto> GetList(int villageId);

        Job GetResourceBuildingJob(int villageId);

        void Move(int jobOldId, int jobNewId);
    }
}