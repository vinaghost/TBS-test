using MainCore.Enums;
using MainCore.Models;
using MainCore.Models.Plans;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MainCore.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        private readonly Dictionary<Type, JobTypeEnums> _jobTypes = new()
        {
            { typeof(NormalBuildPlan),JobTypeEnums.NormalBuild  },
            { typeof(ResourceBuildPlan),JobTypeEnums.ResourceBuild },
        };

        public event Func<int, Task> Locked;

        public event Func<int, Job, Task> AddActionCompleted;

        public event Func<int, Job, Task> DeleteActionCompleted;

        public JobRepository(IDbContextFactory<AppDbContext> contextFactory)

        {
            _contextFactory = contextFactory;
        }

        public async Task Lock(int villageId)
        {
            if (Locked is null) return;
            await Locked(villageId);
        }

        public async Task CompleteAdd(int villageId, Job job)
        {
            if (AddActionCompleted is null) return;
            await AddActionCompleted(villageId, job);
        }

        public async Task CompleteDelete(int villageId, Job job)
        {
            if (DeleteActionCompleted is null) return;
            await DeleteActionCompleted(villageId, job);
        }

        public async Task<Job> Add<T>(int villageId, T content)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var count = await context.Jobs.Where(x => x.VillageId == villageId).CountAsync();
            var job = new Job()
            {
                Position = count,
                VillageId = villageId,
                Type = _jobTypes[typeof(T)],
                Content = JsonSerializer.Serialize(content),
            };
            await context.AddAsync(job);
            await context.SaveChangesAsync();

            return job;
        }

        public async Task<Job> AddToTop<T>(int villageId, T content)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            await context.Jobs
                .Where(x => x.VillageId == villageId)
                .ExecuteUpdateAsync(x =>
                    x.SetProperty(x => x.Position, x => x.Position + 1));

            var job = new Job()
            {
                Position = 0,
                VillageId = villageId,
                Type = _jobTypes[typeof(T)],
                Content = JsonSerializer.Serialize(content),
            };
            await context.AddAsync(job);
            await context.SaveChangesAsync();

            return job;
        }

        public async Task<List<Job>> GetList(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var jobs = await context.Jobs.Where(x => x.VillageId == villageId).OrderBy(x => x.Position).ToListAsync();
            return jobs;
        }

        public async Task<Job> Get(int jobId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var job = await context.Jobs.FindAsync(jobId);
            return job;
        }

        public async Task<int> CountBuildingJob(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var count = await context.Jobs
                .Where(x => x.VillageId == villageId && (x.Type == JobTypeEnums.NormalBuild || x.Type == JobTypeEnums.ResourceBuild))
                .CountAsync();
            return count;
        }

        public async Task<Job> GetResourceBuildingJob(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
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
                .FirstOrDefault();

            var resourceBuildJob = await context.Jobs
                .Where(x => x.VillageId == villageId && x.Type == JobTypeEnums.ResourceBuild)
                .FirstOrDefaultAsync();

            if (job is null) return resourceBuildJob;
            if (resourceBuildJob is null) return job;
            if (job.Position < resourceBuildJob.Position) return job;
            return resourceBuildJob;
        }

        public async Task<Job> GetInfrastructureBuildingJob(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
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
                .FirstOrDefault();
            return job;
        }

        public async Task Move(int jobOldId, int jobNewId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var jobOld = await context.Jobs.FindAsync(jobOldId);
            var jobNew = await context.Jobs.FindAsync(jobNewId);

            (jobNew.Position, jobOld.Position) = (jobOld.Position, jobNew.Position);
            context.Update(jobOld);
            context.Update(jobNew);

            await context.SaveChangesAsync();
        }

        public async Task Delete(int jobId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            await context.Jobs.Where(x => x.Id == jobId).ExecuteDeleteAsync();
        }

        public async Task Clear(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            await context.Jobs.Where(x => x.VillageId == villageId).ExecuteDeleteAsync();
        }

        public async Task<Job> GetFirstJob(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var job = await context.Jobs
                .Where(x => x.VillageId == villageId)
                .OrderBy(x => x.Position)
                .FirstOrDefaultAsync();
            return job;
        }
    }
}