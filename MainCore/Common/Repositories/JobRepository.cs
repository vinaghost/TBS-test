using MainCore.Common.Enums;
using MainCore.Common.Models;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MainCore.Common.Repositories
{
    [RegisterAsTransient]
    public class JobRepository : IJobRepository
    {
        private readonly AppDbContext _context;

        private readonly Dictionary<Type, JobTypeEnums> _jobTypes = new()
        {
            { typeof(NormalBuildPlan),JobTypeEnums.NormalBuild  },
            { typeof(ResourceBuildPlan),JobTypeEnums.ResourceBuild },
        };

        public JobRepository(AppDbContext context)

        {
            _context = context;
        }

        public JobDto Add<T>(int villageId, T content)
        {
            var count = _context.Jobs
                .Where(x => x.VillageId == villageId)
                .Count();
            var job = new Job()
            {
                Position = count,
                VillageId = villageId,
                Type = _jobTypes[typeof(T)],
                Content = JsonSerializer.Serialize(content),
            };
            _context.Add(job);
            _context.SaveChanges();

            var mapper = new JobMapper();
            return mapper.Map(job);
        }

        public Job AddToTop<T>(int villageId, T content)
        {
            _context.Jobs
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
            _context.Add(job);
            _context.SaveChanges();

            return job;
        }

        public List<JobDto> GetList(int villageId)
        {
            var jobs = _context.Jobs
                .Where(x => x.VillageId == villageId)
                .OrderBy(x => x.Position)
                .ProjectToDto()
                .ToList();
            return jobs;
        }

        public Job Get(int jobId)
        {
            var job = _context.Jobs.Find(jobId);
            return job;
        }

        public int CountBuildingJob(int villageId)
        {
            var count = _context.Jobs
                .Where(x => x.VillageId == villageId && (x.Type == JobTypeEnums.NormalBuild || x.Type == JobTypeEnums.ResourceBuild))
                .Count();
            return count;
        }

        public Job GetResourceBuildingJob(int villageId)
        {
            var job = _context.Jobs
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

            var resourceBuildJob = _context.Jobs
                .Where(x => x.VillageId == villageId && x.Type == JobTypeEnums.ResourceBuild)
                .FirstOrDefault();

            if (job is null) return resourceBuildJob;
            if (resourceBuildJob is null) return job;
            if (job.Position < resourceBuildJob.Position) return job;
            return resourceBuildJob;
        }

        public Job GetInfrastructureBuildingJob(int villageId)
        {
            var job = _context.Jobs
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
            var jobOld = _context.Jobs.Find(jobOldId);
            var jobNew = _context.Jobs.Find(jobNewId);

            (jobNew.Position, jobOld.Position) = (jobOld.Position, jobNew.Position);
            _context.Update(jobOld);
            _context.Update(jobNew);

            _context.SaveChanges();
        }

        public void Delete(int jobId)
        {
            _context.Jobs.Where(x => x.Id == jobId).ExecuteDelete();
        }

        public void Clear(int villageId)
        {
            _context.Jobs.Where(x => x.VillageId == villageId).ExecuteDelete();
        }

        public Job GetFirstJob(int villageId)
        {
            var job = _context.Jobs
                .Where(x => x.VillageId == villageId)
                .OrderBy(x => x.Position)
                .FirstOrDefault();
            return job;
        }
    }
}