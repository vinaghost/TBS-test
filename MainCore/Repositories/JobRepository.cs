﻿using MainCore.Enums;
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
            {  typeof(ResourceBuildPlan),JobTypeEnums.ResourceBuild },
        };

        public JobRepository(IDbContextFactory<AppDbContext> contextFactory)

        {
            _contextFactory = contextFactory;
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
    }
}