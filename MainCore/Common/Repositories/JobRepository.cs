using MainCore.Common.Enums;
using MainCore.Common.Models;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MainCore.Common.Repositories
{
    [RegisterAsSingleton]
    public class JobRepository : IJobRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        private readonly Dictionary<Type, JobTypeEnums> _jobTypes = new()
        {
            { typeof(NormalBuildPlan),JobTypeEnums.NormalBuild  },
            { typeof(ResourceBuildPlan),JobTypeEnums.ResourceBuild },
        };

        public JobRepository(IDbContextFactory<AppDbContext> contextFactory)

        {
            _contextFactory = contextFactory;
        }

        public Job Add<T>(int villageId, T content)
        {
            using var context = _contextFactory.CreateDbContext();
            var count = context.Jobs
                .Where(x => x.VillageId == villageId)
                .Count();
            var job = new Job()
            {
                Position = count,
                VillageId = villageId,
                Type = _jobTypes[typeof(T)],
                Content = JsonSerializer.Serialize(content),
            };
            context.Add(job);
            context.SaveChanges();

            return job;
        }

        public Job AddToTop<T>(int villageId, T content)
        {
            using var context = _contextFactory.CreateDbContext();

            context.Jobs
               .Where(x => x.VillageId == villageId)
               .ExecuteUpdate(x =>
                   x.SetProperty(x => x.Position, x => x.Position + 1));

            var job = new Job()
            {
                Position = 0,
                VillageId = villageId,
                Type = _jobTypes[typeof(T)],
                Content = JsonSerializer.Serialize(content),
            };
            context.Add(job);
            context.SaveChanges();

            return job;
        }

        public List<Job> GetList(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var jobs = context.Jobs
                .Where(x => x.VillageId == villageId)
                .OrderBy(x => x.Position)
                .ToList();
            return jobs;
        }

        public Job Get(int jobId)
        {
            using var context = _contextFactory.CreateDbContext();
            var job = context.Jobs.Find(jobId);
            return job;
        }

        public int CountBuildingJob(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var count = context.Jobs
                .Where(x => x.VillageId == villageId && (x.Type == JobTypeEnums.NormalBuild || x.Type == JobTypeEnums.ResourceBuild))
                .Count();
            return count;
        }

        public Job GetResourceBuildingJob(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var job = context.Jobs
                .Where(x => x.VillageId == villageId && x.Type == JobTypeEnums.NormalBuild)
                .AsEnumerable()
                .Select(x => new
                {
                    Job = x,
                    Content = JsonSerializer.Deserialize<NormalBuildPlan>(x.Content)
                })
                .Where(x =>
                    x.Content.Type == BuildingEnums.Woodcutter ||
                    x.Content.Type == BuildingEnums.ClayPit ||
                    x.Content.Type == BuildingEnums.IronMine ||
                    x.Content.Type == BuildingEnums.Cropland)
                .Select(x => x.Job)
                .OrderBy(x => x.Position)
                .FirstOrDefault();

            var resourceBuildJob = context.Jobs
                .Where(x => x.VillageId == villageId && x.Type == JobTypeEnums.ResourceBuild)
                .FirstOrDefault();

            if (job is null) return resourceBuildJob;
            if (resourceBuildJob is null) return job;
            if (job.Position < resourceBuildJob.Position) return job;
            return resourceBuildJob;
        }

        public Job GetInfrastructureBuildingJob(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var job = context.Jobs
                .Where(x => x.VillageId == villageId && x.Type == JobTypeEnums.NormalBuild)
                .AsEnumerable()
                .Select(x => new
                {
                    Job = x,
                    Content = JsonSerializer.Deserialize<NormalBuildPlan>(x.Content)
                })
                .Where(x =>
                    x.Content.Type != BuildingEnums.Woodcutter &&
                    x.Content.Type != BuildingEnums.ClayPit &&
                    x.Content.Type != BuildingEnums.IronMine &&
                    x.Content.Type != BuildingEnums.Cropland)
                .Select(x => x.Job)
                .OrderBy(x => x.Position)
                .FirstOrDefault();
            return job;
        }

        public void Move(int jobOldId, int jobNewId)
        {
            using var context = _contextFactory.CreateDbContext();
            var jobOld = context.Jobs.Find(jobOldId);
            var jobNew = context.Jobs.Find(jobNewId);

            (jobNew.Position, jobOld.Position) = (jobOld.Position, jobNew.Position);
            context.Update(jobOld);
            context.Update(jobNew);

            context.SaveChanges();
        }

        public void Delete(int jobId)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Jobs.Where(x => x.Id == jobId).ExecuteDelete();
        }

        public void Clear(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Jobs.Where(x => x.VillageId == villageId).ExecuteDelete();
        }

        public Job GetFirstJob(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var job = context.Jobs
                .Where(x => x.VillageId == villageId)
                .OrderBy(x => x.Position)
                .FirstOrDefault();
            return job;
        }
    }
}