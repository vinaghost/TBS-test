using MainCore.Common.Enums;
using MainCore.Common.Extensions;
using MainCore.Common.Models;
using MainCore.Common.Repositories;
using MainCore.CQRS.Base;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.Json;

namespace MainCore.CQRS.Queries
{
    public class GetBuildingsWithPlannedLevelQuery : ByVillageIdRequestBase, IRequest<List<BuildingItem>>
    {
        public GetBuildingsWithPlannedLevelQuery(VillageId villageId) : base(villageId)
        {
        }
    }

    public class GetBuildingsWithPlannedLevelQueryHandler : IRequestHandler<GetBuildingsWithPlannedLevelQuery, List<BuildingItem>>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public GetBuildingsWithPlannedLevelQueryHandler(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<BuildingItem>> Handle(GetBuildingsWithPlannedLevelQuery request, CancellationToken cancellationToken)
        {
            var items = await Task.Run(() => Get(request.VillageId), cancellationToken);
            return items;
        }

        private List<BuildingItem> Get(VillageId villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var villageBuildings = context.Buildings
                .Where(x => x.VillageId == villageId)
                .Select(x => new BuildingItem()
                {
                    Location = x.Location,
                    Type = x.Type,
                    Level = x.Level,
                })
                .AsEnumerable();

            var queueBuildings = context.QueueBuildings
                .Where(x => x.VillageId == villageId)
                .Where(x => x.Type != BuildingEnums.Site)
                .GroupBy(x => x.Location)
                .Select(x => new BuildingItem()
                {
                    Location = x.Key,
                    Type = x.OrderBy(x => x.Id).Select(x => x.Type).First(),
                    Level = x.OrderByDescending(x => x.Location).Select(x => x.Level).First(),
                })
                .AsEnumerable();

            var jobBuildings = context.Jobs
                .Where(x => x.VillageId == villageId)
                .Where(x => x.Type == JobTypeEnums.NormalBuild)
                .AsEnumerable()
                .Select(x => JsonSerializer.Deserialize<NormalBuildPlan>(x.Content))
                .GroupBy(x => x.Location)
                .Select(x => new BuildingItem()
                {
                    Location = x.Key,
                    Type = x.OrderBy(x => x.Location).Select(x => x.Type).First(),
                    Level = x.OrderByDescending(x => x.Location).Select(x => x.Level).First(),
                });

            var buildings = new[] { jobBuildings, queueBuildings, villageBuildings }
                .SelectMany(x => x)
                .GroupBy(x => x.Location)
                .Select(x => new BuildingItem()
                {
                    Location = x.Key,
                    Type = x.OrderBy(x => x.Location).Select(x => x.Type).First(),
                    Level = x.OrderByDescending(x => x.Location).Select(x => x.Level).First(),
                })
                .OrderBy(x => x.Location)
                .ToList();

            var resourceJobs = context.Jobs
               .Where(x => x.VillageId == villageId)
               .Where(x => x.Type == JobTypeEnums.ResourceBuild)
               .AsEnumerable()
               .Select(x => JsonSerializer.Deserialize<ResourceBuildPlan>(x.Content))
               .GroupBy(x => x.Plan)
               .Select(x => new ResourceBuildPlan
               {
                   Plan = x.Key,
                   Level = x.OrderByDescending(x => x.Level).Select(x => x.Level).First(),
               })
               .ToDictionary(x => x.Plan, x => x.Level);
            foreach (var building in buildings)
            {
                if (!building.Type.IsResourceField()) continue;
                foreach (var job in resourceJobs)
                {
                    if (job.Key == ResourcePlanEnums.AllResources)
                    {
                        building.Level = building.Level < job.Value ? job.Value : building.Level;
                        continue;
                    }
                    if (job.Key == ResourcePlanEnums.ExcludeCrop)
                    {
                        if (building.Type == BuildingEnums.Cropland) continue;
                        building.Level = building.Level < job.Value ? job.Value : building.Level;
                        continue;
                    }
                    if (job.Key == ResourcePlanEnums.OnlyCrop)
                    {
                        if (building.Type != BuildingEnums.Cropland) continue;
                        building.Level = building.Level < job.Value ? job.Value : building.Level;
                        continue;
                    }
                }
            }
            return buildings;
        }
    }
}