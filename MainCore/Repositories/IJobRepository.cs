﻿using MainCore.DTO;
using MainCore.Entities;
using MainCore.UI.Models.Output;

namespace MainCore.Repositories
{
    public interface IJobRepository
    {
        void Add<T>(VillageId villageId, T content);

        void AddToTop<T>(VillageId villageId, T content);

        int CountBuildingJob(VillageId villageId);

        void Delete(VillageId villageId);

        void Delete(JobId jobId);

        JobDto GetBuildingJob(VillageId villageId);

        JobDto GetInfrastructureBuildingJob(VillageId villageId);

        List<ListBoxItem> GetItems(VillageId villageId);

        JobDto GetResourceBuildingJob(VillageId villageId);

        void Move(JobId oldJobId, JobId newJobId);
    }
}