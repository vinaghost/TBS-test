using FluentResults;
using Humanizer;
using MainCore.Common.Enums;
using MainCore.Common.Extensions;
using MainCore.Common.Models;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MainCore.Common.Repositories
{
    [RegisterAsSingleton]
    public class BuildingRepository : IBuildingRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public event Func<int, Task> BuildingUpdated;

        private readonly List<BuildingEnums> _availableBuildings;

        public BuildingRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;

            _availableBuildings = new();
            for (var i = BuildingEnums.Sawmill; i <= BuildingEnums.Hospital; i++)
            {
                _availableBuildings.Add(i);
            }
        }

        public List<BuildingEnums> GetAvailableBuildings()
        {
            return _availableBuildings;
        }

        public async Task<List<Building>> GetList(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var buildings = await context.Buildings.Where(x => x.VillageId == villageId).OrderBy(x => x.Location).ToListAsync();
            return buildings;
        }

        public async Task<Building> Get(int buildingId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var building = await context.Buildings.FindAsync(buildingId);
            return building;
        }

        public async Task<Building> GetBasedOnLocation(int villageId, int location)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var building = await context.Buildings
                .Where(x => x.VillageId == villageId && x.Location == location)
                .FirstOrDefaultAsync();
            return building;
        }

        public async Task<int> CountQueueBuilding(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var count = await context.QueueBuildings
                .Where(x => x.VillageId == villageId && x.Type != BuildingEnums.Site)
                .CountAsync();
            return count;
        }

        public async Task<int> CountResourceQueueBuilding(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var count = await context.QueueBuildings
                .Where(x => x.VillageId == villageId)
                .Where(x =>
                    x.Type == BuildingEnums.Woodcutter ||
                    x.Type == BuildingEnums.ClayPit ||
                    x.Type == BuildingEnums.IronMine ||
                    x.Type == BuildingEnums.Cropland)
                .CountAsync();
            return count;
        }

        public async Task<int> CountInfrastructureQueueBuilding(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var count = await context.QueueBuildings
                .Where(x => x.VillageId == villageId && x.Type != BuildingEnums.Site)
                .Where(x =>
                    x.Type != BuildingEnums.Woodcutter &&
                    x.Type != BuildingEnums.ClayPit &&
                    x.Type != BuildingEnums.IronMine &&
                    x.Type != BuildingEnums.Cropland)
                .CountAsync();
            return count;
        }

        public async Task<bool> HasRallyPoint(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Buildings
                        .Where(x => x.VillageId == villageId && x.Level > 0 && x.Type == BuildingEnums.RallyPoint)
                        .AnyAsync();
        }

        public async Task<bool> IsValid(int villageId, Job job)
        {
            if (job.Type == JobTypeEnums.ResourceBuild) return true;
            var plan = JsonSerializer.Deserialize<NormalBuildPlan>(job.Content);
            using var context = await _contextFactory.CreateDbContextAsync();
            var building = await context.Buildings.FirstOrDefaultAsync(x => x.VillageId == villageId && x.Location == plan.Location);
            if (building is null) return true;
            if (building.Level >= plan.Level) return false;

            var queueBuilding = await context.QueueBuildings
                .Where(x => x.VillageId == villageId && x.Location == plan.Location)
                .OrderByDescending(x => x.Level)
                .FirstOrDefaultAsync();
            if (queueBuilding is null) return true;
            if (queueBuilding.Level >= plan.Level) return false;
            return true;
        }

        public async Task Update(int villageId, List<Building> buildings)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var dbBuildings = await context.Buildings.Where(x => x.VillageId == villageId).OrderBy(x => x.Location).ToListAsync();
                foreach (var building in buildings)
                {
                    var dbBuilding = dbBuildings.FirstOrDefault(x => x.Location == building.Location);
                    if (dbBuilding is null)
                    {
                        await context.AddAsync(building);
                    }
                    else
                    {
                        dbBuilding.Level = building.Level;
                        dbBuilding.Type = building.Type;
                        dbBuilding.IsUnderConstruction = building.IsUnderConstruction;
                        context.Update(dbBuilding);
                    }
                }
                await context.SaveChangesAsync();
            }
            if (BuildingUpdated is not null)
            {
                await BuildingUpdated(villageId);
            }
        }

        public async Task<Building> GetCropland(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var building = await context.Buildings
                .Where(x => x.VillageId == villageId
                            && x.Type == BuildingEnums.Cropland)
                .OrderByDescending(x => x.Level)
                .FirstOrDefaultAsync();
            return building;
        }

        public async Task<NormalBuildPlan> GetNormalBuildPlan(int villageId, ResourceBuildPlan plan)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.Buildings.Where(x => x.VillageId == villageId);
            switch (plan.Plan)
            {
                case ResourcePlanEnums.AllResources:
                    query = query.Where(x =>
                    x.Type == BuildingEnums.Woodcutter ||
                    x.Type == BuildingEnums.ClayPit ||
                    x.Type == BuildingEnums.IronMine ||
                    x.Type == BuildingEnums.Cropland);
                    break;

                case ResourcePlanEnums.ExcludeCrop:
                    query = query.Where(x =>
                    x.Type == BuildingEnums.Woodcutter ||
                    x.Type == BuildingEnums.ClayPit ||
                    x.Type == BuildingEnums.IronMine);
                    break;

                case ResourcePlanEnums.OnlyCrop:
                    query = query.Where(x =>
                    x.Type == BuildingEnums.Cropland);
                    break;

                default:
                    break;
            }

            var buildings = query.AsEnumerable();
            foreach (var building in buildings)
            {
                if (building.IsUnderConstruction)
                {
                    var levelUpgrading = await context.QueueBuildings.Where(x => x.VillageId == villageId && x.Location == building.Location).CountAsync();
                    building.Level += levelUpgrading;
                }
            }

            buildings = buildings.Where(x => x.Level < plan.Level);
            if (!buildings.Any()) return null;
            var chosenOne = buildings.OrderBy(x => x.Level).FirstOrDefault();

            var normalBuildPlan = new NormalBuildPlan()
            {
                Type = chosenOne.Type,
                Level = chosenOne.Level + 1,
                Location = chosenOne.Location,
            };
            return normalBuildPlan;
        }

        public async Task<Result> CheckRequirements(int villageId, NormalBuildPlan plan)
        {
            var prerequisiteBuildings = plan.Type.GetPrerequisiteBuildings();
            if (prerequisiteBuildings.Count == 0) return Result.Ok();
            var buildings = await GetBuildings(villageId);
            foreach (var prerequisiteBuilding in prerequisiteBuildings)
            {
                var building = buildings.FirstOrDefault(x => x.Type == prerequisiteBuilding.Type);
                if (building is null) return Result.Fail($"Required {prerequisiteBuilding.Type.Humanize()} lvl {prerequisiteBuilding.Level}");
                if (building.Level < prerequisiteBuilding.Level) return Result.Fail($"Required {prerequisiteBuilding.Type.Humanize()} lvl {prerequisiteBuilding.Level}");
            }
            return Result.Ok();
        }

        public async Task Validate(int villageId, NormalBuildPlan plan)
        {
            if (plan.Type.IsWall())
            {
                plan.Location = 40;
                return;
            }
            var buildings = await GetBuildings(villageId);
            if (plan.Type.IsMultipleBuilding())
            {
                var sameTypeBuildings = buildings.Where(x => x.Type == plan.Type);
                if (!sameTypeBuildings.Any()) return;
                if (sameTypeBuildings.Where(x => x.Location == plan.Location).Any()) return;
                var largestLevelBuilding = sameTypeBuildings.MaxBy(x => x.Level);
                if (largestLevelBuilding.Level == plan.Type.GetMaxLevel()) return;
                plan.Location = largestLevelBuilding.Location;
                return;
            }

            if (plan.Type.IsResourceField())
            {
                var field = buildings.FirstOrDefault(x => x.Location == plan.Location);
                if (plan.Type == field.Type) return;
                plan.Type = field.Type;
            }

            {
                var building = buildings.FirstOrDefault(x => x.Type == plan.Type);
                if (building is null) return;
                if (plan.Location == building.Location) return;
                plan.Location = building.Location;
            }
        }

        public async Task<List<BuildingItemDto>> GetBuildingItems(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var villageBuildings = context.Buildings
                .Where(x => x.VillageId == villageId)
                .OrderBy(x => x.Location)
                .Select(x => new BuildingItemDto()
                {
                    Id = x.Id,
                    Location = x.Location,
                    Type = x.Type,
                    Level = x.Level,
                })
                .ToList();

            var queueBuildings = context.QueueBuildings
                .Where(x => x.VillageId == villageId && x.Type != BuildingEnums.Site)
                .GroupBy(x => x.Location)
                .AsEnumerable()
                .Select(x => new Building()
                {
                    Location = x.Key,
                    Type = x.First().Type,
                    Level = x.MaxBy(x => x.Level).Level,
                });

            foreach (var queueBuilding in queueBuildings)
            {
                var villageBuilding = villageBuildings.FirstOrDefault(x => x.Location == queueBuilding.Location);
                if (villageBuilding is null) continue;
                villageBuilding.QueueLevel = queueBuilding.Level;
            }

            var jobBuildings = context.Jobs
                .Where(x => x.VillageId == villageId && x.Type == JobTypeEnums.NormalBuild)
                .AsEnumerable()
                .Select(x => JsonSerializer.Deserialize<NormalBuildPlan>(x.Content))
                .GroupBy(x => x.Location)
                .Select(x => new Building()
                {
                    Location = x.Key,
                    Type = x.First().Type,
                    Level = x.MaxBy(x => x.Level).Level,
                })
                .AsEnumerable();

            foreach (var jobBuilding in jobBuildings)
            {
                var villageBuilding = villageBuildings.FirstOrDefault(x => x.Location == jobBuilding.Location);
                if (villageBuilding is null) continue;
                villageBuilding.JobLevel = jobBuilding.Level;
            }

            var resourceJobs = context.Jobs
               .Where(x => x.VillageId == villageId && x.Type == JobTypeEnums.ResourceBuild)
               .AsEnumerable()
               .Select(x => JsonSerializer.Deserialize<ResourceBuildPlan>(x.Content))
               .GroupBy(x => x.Plan)
               .Select(x => new ResourceBuildPlan
               {
                   Plan = x.Key,
                   Level = x.MaxBy(x => x.Level).Level,
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

            return villageBuildings;
        }

        private async Task<List<BuildingItem>> GetBuildings(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var villageBuildings = context.Buildings
                .Where(x => x.VillageId == villageId)
                .OrderBy(x => x.Location)
                .Select(x => new BuildingItem()
                {
                    Location = x.Location,
                    Type = x.Type,
                    Level = x.Level,
                })
                .AsEnumerable();

            var queueBuildings = context.QueueBuildings
                .Where(x => x.VillageId == villageId && x.Type != BuildingEnums.Site)
                .GroupBy(x => x.Location)
                .AsEnumerable()
                .Select(x => new BuildingItem()
                {
                    Location = x.Key,
                    Type = x.First().Type,
                    Level = x.MaxBy(x => x.Level).Level,
                });

            var jobBuildings = context.Jobs
                .Where(x => x.VillageId == villageId && x.Type == JobTypeEnums.NormalBuild)
                .AsEnumerable()
                .Select(x => JsonSerializer.Deserialize<NormalBuildPlan>(x.Content))
                .GroupBy(x => x.Location)
                .Select(x => new BuildingItem()
                {
                    Location = x.Key,
                    Type = x.First().Type,
                    Level = x.MaxBy(x => x.Level).Level,
                })
                .AsEnumerable();

            var buildings = new[] { jobBuildings, queueBuildings, villageBuildings }
                .SelectMany(x => x)
                .GroupBy(x => x.Location)
                .Select(x => new BuildingItem()
                {
                    Location = x.Key,
                    Type = x.First().Type,
                    Level = x.MaxBy(x => x.Level).Level,
                })
                .OrderBy(x => x.Location)
                .ToList();

            var resourceJobs = context.Jobs
               .Where(x => x.VillageId == villageId && x.Type == JobTypeEnums.ResourceBuild)
               .AsEnumerable()
               .Select(x => JsonSerializer.Deserialize<ResourceBuildPlan>(x.Content))
               .GroupBy(x => x.Plan)
               .Select(x => new ResourceBuildPlan
               {
                   Plan = x.Key,
                   Level = x.MaxBy(x => x.Level).Level,
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

        private class BuildingItem
        {
            public int Location { get; set; }
            public BuildingEnums Type { get; set; }
            public int Level { get; set; }
        }
    }
}