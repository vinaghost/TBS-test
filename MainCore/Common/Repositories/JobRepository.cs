using MainCore.Common.Enums;
using MainCore.Common.Models;
using MainCore.Common.Notification;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MainCore.Common.Repositories
{
    [RegisterAsTransient]
    public class JobRepository : IJobRepository
    {
        private readonly AppDbContext _context;
        private readonly IMediator _mediator;

        private readonly Dictionary<Type, JobTypeEnums> _jobTypes = new()
        {
            { typeof(NormalBuildPlan),JobTypeEnums.NormalBuild  },
            { typeof(ResourceBuildPlan),JobTypeEnums.ResourceBuild },
        };

        public JobRepository(AppDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task Add<T>(int villageId, T content)
        {
            var count = await Task.Run(() =>
                _context.Jobs
                    .Where(x => x.VillageId == villageId)
                    .Count());
            var job = new Job()
            {
                Position = count,
                VillageId = villageId,
                Type = _jobTypes[typeof(T)],
                Content = JsonSerializer.Serialize(content),
            };
            _context.Add(job);
            await Task.Run(() => _context.SaveChanges());
            await _mediator.Publish(new JobUpdated(villageId));
        }

        public async Task AddToTop<T>(int villageId, T content)
        {
            await Task.Run(() =>
                _context.Jobs
                   .Where(x => x.VillageId == villageId)
                   .ExecuteUpdate(x =>
                       x.SetProperty(x => x.Position, x => x.Position + 1)));

            var job = new Job()
            {
                Position = 0,
                VillageId = villageId,
                Type = _jobTypes[typeof(T)],
                Content = JsonSerializer.Serialize(content),
            };
            _context.Add(job);
            await Task.Run(() => _context.SaveChanges());
            await _mediator.Publish(new JobUpdated(villageId));
        }

        public async Task<List<JobDto>> GetAll(int villageId)
        {
            var jobs = await Task.Run(() =>
                _context.Jobs
                    .Where(x => x.VillageId == villageId)
                    .OrderBy(x => x.Position)
                    .ProjectToDto()
                    .ToList());
            return jobs;
        }

        public async Task<JobDto> GetById(int jobId)
        {
            var job = await Task.Run(() => _context.Jobs.Find(jobId));
            var mapper = new JobMapper();
            var dto = mapper.Map(job);
            return dto;
        }

        public async Task<int> CountBuildingJob(int villageId)
        {
            var count = await Task.Run(() =>
                _context.Jobs
                    .Where(x => x.VillageId == villageId && (x.Type == JobTypeEnums.NormalBuild || x.Type == JobTypeEnums.ResourceBuild))
                    .Count());
            return count;
        }

        public async Task<JobDto> GetResourceBuildingJob(int villageId)
        {
            var job = await Task.Run(() =>
                _context.Jobs
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
                    .FirstOrDefault());

            var resourceBuildJob = await Task.Run(() =>
                _context.Jobs
                    .Where(x => x.VillageId == villageId && x.Type == JobTypeEnums.ResourceBuild)
                    .FirstOrDefault());
            var mapper = new JobMapper();

            if (job is null) return mapper.Map(resourceBuildJob);
            if (resourceBuildJob is null) return mapper.Map(job);
            if (job.Position < resourceBuildJob.Position) return mapper.Map(job);
            return mapper.Map(resourceBuildJob);
        }

        public async Task<JobDto> GetInfrastructureBuildingJob(int villageId)
        {
            var job = await Task.Run(() =>
                _context.Jobs
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
                    .FirstOrDefault());
            var mapper = new JobMapper();
            return mapper.Map(job);
        }

        public async Task Move(int jobOldId, int jobNewId)
        {
            var jobOld = await Task.Run(() => _context.Jobs.Find(jobOldId));
            var jobNew = await Task.Run(() => _context.Jobs.Find(jobNewId));

            (jobNew.Position, jobOld.Position) = (jobOld.Position, jobNew.Position);
            _context.Update(jobOld);
            _context.Update(jobNew);

            await Task.Run(_context.SaveChanges);
        }

        public async Task Delete(int jobId)
        {
            await Task.Run(_context.Jobs.Where(x => x.Id == jobId).ExecuteDelete);
        }

        public async Task Clear(int villageId)
        {
            await Task.Run(_context.Jobs.Where(x => x.VillageId == villageId).ExecuteDelete);
            await _mediator.Publish(new JobUpdated(villageId));
        }

        public async Task<JobDto> GetFirstJob(int villageId)
        {
            var job = await Task.Run(() =>
                _context.Jobs
                    .Where(x => x.VillageId == villageId)
                    .OrderBy(x => x.Position)
                    .FirstOrDefault());
            var mapper = new JobMapper();
            var dto = mapper.Map(job);
            return dto;
        }
    }
}