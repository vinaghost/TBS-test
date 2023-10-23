using FluentResults;
using Humanizer;
using MainCore.Common.Enums;
using MainCore.Common.Extensions;
using MainCore.Common.Models;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using System.Text.Json;

namespace MainCore.Common.Repositories
{
    [RegisterAsTransient]
    public class BuildingRepository : IBuildingRepository
    {
        private readonly AppDbContext _context;

        private readonly List<BuildingEnums> _availableBuildings = new();

        public BuildingRepository(AppDbContext context)
        {
            _context = context;

            for (var i = BuildingEnums.Sawmill; i <= BuildingEnums.Hospital; i++)
            {
                _availableBuildings.Add(i);
            }
        }

        public List<BuildingEnums> AvailableBuildings => _availableBuildings;

        public List<Building> GetBuildingList(int villageId)
        {
            var buildings = _context.Buildings
                .Where(x => x.VillageId == villageId)
                .OrderBy(x => x.Location)
                .ToList();
            return buildings;
        }

        public Building GetBuilding(int buildingId)
        {
            var building = _context.Buildings
                .Find(buildingId);
            return building;
        }

        public Building GetBuilding(int villageId, int location)
        {
            var building = _context.Buildings
                .Where(x => x.VillageId == villageId && x.Location == location)
                .FirstOrDefault();
            return building;
        }

        public int CountQueueBuilding(int villageId)
        {
            var count = _context.QueueBuildings
                .Where(x => x.VillageId == villageId && x.Type != BuildingEnums.Site)
                .Count();
            return count;
        }

        public int CountResourceQueueBuilding(int villageId)
        {
            var count = _context.QueueBuildings
                .Where(x => x.VillageId == villageId)
                .Where(x =>
                    x.Type == BuildingEnums.Woodcutter ||
                    x.Type == BuildingEnums.ClayPit ||
                    x.Type == BuildingEnums.IronMine ||
                    x.Type == BuildingEnums.Cropland)
                .Count();
            return count;
        }

        public bool HasRallyPoint(int villageId)
        {
            return _context.Buildings
                        .Where(x => x.VillageId == villageId && x.Level > 0 && x.Type == BuildingEnums.RallyPoint)
                        .Any();
        }

        public bool IsJobValid(int villageId, JobDto job)
        {
            if (job.Type == JobTypeEnums.ResourceBuild) return true;
            var plan = JsonSerializer.Deserialize<NormalBuildPlan>(job.Content);

            var building = _context.Buildings.FirstOrDefault(x => x.VillageId == villageId && x.Location == plan.Location);
            if (building is null) return true;
            if (building.Level >= plan.Level) return false;

            var queueBuilding = _context.QueueBuildings
                .Where(x => x.VillageId == villageId && x.Location == plan.Location)
                .OrderByDescending(x => x.Level)
                .FirstOrDefault();
            if (queueBuilding is null) return true;
            if (queueBuilding.Level >= plan.Level) return false;
            return true;
        }

        public void Update(int villageId, List<Building> buildings)
        {
            var dbBuildings = _context.Buildings
                .Where(x => x.VillageId == villageId)
                .OrderBy(x => x.Location)
                .ToList();
            foreach (var building in buildings)
            {
                var dbBuilding = dbBuildings
                    .FirstOrDefault(x => x.Location == building.Location);
                if (dbBuilding is null)
                {
                    _context.Add(building);
                }
                else
                {
                    dbBuilding.Level = building.Level;
                    dbBuilding.Type = building.Type;
                    dbBuilding.IsUnderConstruction = building.IsUnderConstruction;
                    _context.Update(dbBuilding);
                }
            }
            _context.SaveChanges();
        }

        public Building GetCropland(int villageId)
        {
            var building = _context.Buildings
                .Where(x => x.VillageId == villageId
                            && x.Type == BuildingEnums.Cropland)
                .OrderByDescending(x => x.Level)
                .FirstOrDefault();
            return building;
        }

        public NormalBuildPlan GetNormalBuildPlan(int villageId, ResourceBuildPlan plan)
        {
            var query = _context.Buildings.Where(x => x.VillageId == villageId);
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
                    var levelUpgrading = _context.QueueBuildings.Where(x => x.VillageId == villageId && x.Location == building.Location).Count();
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

        public Result CheckRequirements(int villageId, NormalBuildPlan plan)
        {
            var prerequisiteBuildings = plan.Type.GetPrerequisiteBuildings();
            if (prerequisiteBuildings.Count == 0) return Result.Ok();
            var buildings = GetBuildings(villageId);
            foreach (var prerequisiteBuilding in prerequisiteBuildings)
            {
                var building = buildings.FirstOrDefault(x => x.Type == prerequisiteBuilding.Type);
                if (building is null) return Result.Fail($"Required {prerequisiteBuilding.Type.Humanize()} lvl {prerequisiteBuilding.Level}");
                if (building.Level < prerequisiteBuilding.Level) return Result.Fail($"Required {prerequisiteBuilding.Type.Humanize()} lvl {prerequisiteBuilding.Level}");
            }
            return Result.Ok();
        }

        public void Validate(int villageId, NormalBuildPlan plan)
        {
            if (plan.Type.IsWall())
            {
                plan.Location = 40;
                return;
            }
            var buildings = GetBuildings(villageId);
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

        public List<BuildingItemDto> GetBuildingItems(int villageId)
        {
            var villageBuildings = _context.Buildings
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

            var queueBuildings = _context.QueueBuildings
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

            var jobBuildings = _context.Jobs
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

            var resourceJobs = _context.Jobs
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

        private List<BuildingItem> GetBuildings(int villageId)
        {
            var villageBuildings = _context.Buildings
                .Where(x => x.VillageId == villageId)
                .OrderBy(x => x.Location)
                .Select(x => new BuildingItem()
                {
                    Location = x.Location,
                    Type = x.Type,
                    Level = x.Level,
                })
                .AsEnumerable();

            var queueBuildings = _context.QueueBuildings
                .Where(x => x.VillageId == villageId && x.Type != BuildingEnums.Site)
                .GroupBy(x => x.Location)
                .AsEnumerable()
                .Select(x => new BuildingItem()
                {
                    Location = x.Key,
                    Type = x.First().Type,
                    Level = x.MaxBy(x => x.Level).Level,
                });

            var jobBuildings = _context.Jobs
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

            var resourceJobs = _context.Jobs
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