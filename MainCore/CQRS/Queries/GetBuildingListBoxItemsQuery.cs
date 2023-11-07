using Humanizer;
using MainCore.Common.Enums;
using MainCore.Common.Extensions;
using MainCore.Common.Models;
using MainCore.CQRS.Base;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MainCore.UI.Models.Output;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace MainCore.CQRS.Queries
{
    public class GetBuildingListBoxItemsQuery : ByVillageIdRequestBase, IRequest<List<ListBoxItem>>
    {
        public GetBuildingListBoxItemsQuery(VillageId villageId) : base(villageId)
        {
        }
    }

    public class GetBuildingListBoxItemsQueryHandler : IRequestHandler<GetBuildingListBoxItemsQuery, List<ListBoxItem>>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public GetBuildingListBoxItemsQueryHandler(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<ListBoxItem>> Handle(GetBuildingListBoxItemsQuery request, CancellationToken cancellationToken)
        {
            var items = await Task.Run(() => GetBuildingItems(request.VillageId), cancellationToken);
            return items;
        }

        public List<ListBoxItem> GetBuildingItems(VillageId villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var villageBuildings = context.Buildings
                .AsNoTracking()
                .Where(x => x.VillageId == villageId.Value)
                .OrderBy(x => x.Location)
                .AsEnumerable()
                .Select(x => new BuildingItemDto()
                {
                    Id = new BuildingId(x.Id),
                    Location = x.Location,
                    Type = x.Type,
                    Level = x.Level,
                })
                .ToList();

            var queueBuildings = context.QueueBuildings
                .AsNoTracking()
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Type != BuildingEnums.Site)
                .GroupBy(x => x.Location)
                .Select(x => new Building()
                {
                    Location = x.Key,
                    Type = x.OrderBy(x => x.Id).Select(x => x.Type).First(),
                    Level = x.OrderBy(x => x.Id).Select(x => x.Level).Max(),
                })
                .AsEnumerable();

            foreach (var queueBuilding in queueBuildings)
            {
                var villageBuilding = villageBuildings
                    .Where(x => x.Location == queueBuilding.Location)
                    .FirstOrDefault();
                if (villageBuilding is null) continue;
                villageBuilding.QueueLevel = queueBuilding.Level;
            }

            var jobBuildings = context.Jobs
                .AsNoTracking()
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Type == JobTypeEnums.NormalBuild)
                .Select(x => x.Content)
                .AsEnumerable()
                .Select(x => JsonSerializer.Deserialize<NormalBuildPlan>(x))
                .GroupBy(x => x.Location)
                .Select(x => new Building()
                {
                    Location = x.Key,
                    Type = x.OrderBy(x => x.Level).Select(x => x.Type).First(),
                    Level = x.OrderByDescending(x => x.Level).Select(x => x.Level).First(),
                });

            foreach (var jobBuilding in jobBuildings)
            {
                var villageBuilding = villageBuildings
                    .Where(x => x.Location == jobBuilding.Location)
                    .FirstOrDefault();
                if (villageBuilding is null) continue;
                villageBuilding.JobLevel = jobBuilding.Level;
            }

            var resourceJobs = context.Jobs
               .AsNoTracking()
               .Where(x => x.VillageId == villageId.Value)
               .Where(x => x.Type == JobTypeEnums.ResourceBuild)
               .Select(x => x.Content)
               .AsEnumerable()
               .Select(x => JsonSerializer.Deserialize<ResourceBuildPlan>(x))
               .GroupBy(x => x.Plan)
               .Select(x => new ResourceBuildPlan
               {
                   Plan = x.Key,
                   Level = x.OrderByDescending(x => x.Level).Select(x => x.Level).First(),
               })
               .ToDictionary(x => x.Plan, x => x.Level);
            foreach (var building in villageBuildings)
            {
                if (!building.Type.IsResourceField()) continue;
                foreach (var job in resourceJobs)
                {
                    switch (job.Key)
                    {
                        case ResourcePlanEnums.AllResources:
                            building.JobLevel = building.JobLevel < job.Value ? job.Value : building.JobLevel;
                            break;

                        case ResourcePlanEnums.ExcludeCrop:
                            if (building.Type == BuildingEnums.Cropland) break;
                            building.JobLevel = building.JobLevel < job.Value ? job.Value : building.JobLevel;
                            break;

                        case ResourcePlanEnums.OnlyCrop:
                            if (building.Type != BuildingEnums.Cropland) break;
                            building.JobLevel = building.JobLevel < job.Value ? job.Value : building.JobLevel;
                            break;

                        default:
                            break;
                    }
                }
            }

            var items = villageBuildings
                .Select(x => ToListBoxItem(x))
                .ToList();
            return items;
        }

        private static ListBoxItem ToListBoxItem(BuildingItemDto building)
        {
            const string arrow = " -> ";
            var sb = new StringBuilder();
            sb.Append(building.Level);
            if (building.QueueLevel != 0)
            {
                var content = $"{arrow}({building.QueueLevel})";
                sb.Append(content);
            }
            if (building.JobLevel != 0)
            {
                var content = $"{arrow}[{building.JobLevel}]";
                sb.Append(content);
            }

            var item = new ListBoxItem()
            {
                Id = building.Id.Value,
                Content = $"[{building.Location}] {building.Type.Humanize()} | lvl {sb}",
                Color = building.Type.GetColor(),
            };
            return item;
        }
    }
}