using FluentResults;
using Humanizer;
using MainCore;
using MainCore.Enums;
using MainCore.Models.Plans;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WPFUI.Models.Output;

namespace WPFUI.Repositories
{
    public class BuildRepository : IBuildRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        private readonly List<BuildingEnums> _availableBuildings;

        public BuildRepository(IDbContextFactory<AppDbContext> contextFactory)
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

        public async Task<List<BuildingItem>> GetBuildingItems(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var villageBuildings = context.Buildings
                .Where(x => x.VillageId == villageId)
                .OrderBy(x => x.Location)
                .Select(x => new BuildingItem()
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

        private async Task<List<Building>> GetBuildings(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var villageBuildings = context.Buildings
                .Where(x => x.VillageId == villageId)
                .OrderBy(x => x.Location)
                .Select(x => new Building()
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
                .Select(x => new Building()
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
                .Select(x => new Building()
                {
                    Location = x.Key,
                    Type = x.First().Type,
                    Level = x.MaxBy(x => x.Level).Level,
                })
                .AsEnumerable();

            var buildings = new[] { jobBuildings, queueBuildings, villageBuildings }
                .SelectMany(x => x)
                .GroupBy(x => x.Location)
                .Select(x => new Building()
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

        private class Building
        {
            public int Location { get; set; }
            public BuildingEnums Type { get; set; }
            public int Level { get; set; }
        }
    }

    public static class BuildExtension
    {
        public static bool IsWall(this BuildingEnums building)
        {
            return building switch
            {
                BuildingEnums.EarthWall => true,
                BuildingEnums.CityWall => true,
                BuildingEnums.Palisade => true,
                BuildingEnums.StoneWall => true,
                BuildingEnums.MakeshiftWall => true,
                _ => false,
            };
        }

        public static bool IsMultipleBuilding(this BuildingEnums building)
        {
            return building switch
            {
                BuildingEnums.Warehouse => true,
                BuildingEnums.Granary => true,
                BuildingEnums.GreatWarehouse => true,
                BuildingEnums.GreatGranary => true,
                BuildingEnums.Trapper => true,
                BuildingEnums.Cranny => true,
                _ => false,
            };
        }

        public static int GetMaxLevel(this BuildingEnums building)
        {
            return building switch
            {
                BuildingEnums.Bakery => 5,
                BuildingEnums.Brickyard => 5,
                BuildingEnums.IronFoundry => 5,
                BuildingEnums.GrainMill => 5,
                BuildingEnums.Sawmill => 5,

                BuildingEnums.Cranny => 10,
                _ => 20,
            };
        }

        public static List<PrerequisiteBuilding> GetPrerequisiteBuildings(this BuildingEnums building)
        {
            return building switch
            {
                BuildingEnums.Sawmill => new()
                    {
                        new(BuildingEnums.Woodcutter, 10),
                        new(BuildingEnums.MainBuilding, 5),
                    },
                BuildingEnums.Brickyard => new()
                    {
                        new(BuildingEnums.ClayPit, 10),
                        new(BuildingEnums.MainBuilding, 5),
                    },
                BuildingEnums.IronFoundry => new()
                    {
                        new(BuildingEnums.IronMine, 10),
                        new(BuildingEnums.MainBuilding, 5),
                    },
                BuildingEnums.GrainMill => new()
                    {
                        new(BuildingEnums.Cropland, 10),
                        new(BuildingEnums.MainBuilding, 5),
                    },
                BuildingEnums.Bakery => new()
                    {
                        new(BuildingEnums.Cropland, 10),
                        new(BuildingEnums.GrainMill, 10),
                        new(BuildingEnums.MainBuilding, 5),
                    },
                BuildingEnums.Warehouse => new()
                    {
                        new(BuildingEnums.MainBuilding, 1),
                    },
                BuildingEnums.Granary => new()
                    {
                        new(BuildingEnums.MainBuilding, 1),
                    },
                BuildingEnums.Smithy => new()
                    {
                        new(BuildingEnums.MainBuilding, 3),
                        new(BuildingEnums.Academy, 1),
                    },
                BuildingEnums.TournamentSquare => new()
                    {
                        new(BuildingEnums.RallyPoint, 15),
                    },
                BuildingEnums.Marketplace => new()
                    {
                        new(BuildingEnums.Warehouse, 1),
                        new(BuildingEnums.Granary, 1),
                        new(BuildingEnums.MainBuilding, 3),
                    },
                BuildingEnums.Embassy => new()
                    {
                        new(BuildingEnums.MainBuilding, 1),
                    },
                BuildingEnums.Barracks => new()
                    {
                        new(BuildingEnums.RallyPoint, 1),
                        new(BuildingEnums.MainBuilding, 3),
                    },
                BuildingEnums.Stable => new()
                    {
                        new(BuildingEnums.Academy, 5),
                        new(BuildingEnums.Smithy, 3),
                    },
                BuildingEnums.Workshop => new()
                    {
                        new(BuildingEnums.Academy, 10),
                        new(BuildingEnums.MainBuilding, 5),
                    },
                BuildingEnums.Academy => new()
                    {
                        new(BuildingEnums.Barracks, 3),
                        new(BuildingEnums.MainBuilding, 3),
                    },
                BuildingEnums.TownHall => new()
                    {
                        new(BuildingEnums.Academy, 10),
                        new(BuildingEnums.MainBuilding, 10),
                    },
                BuildingEnums.Residence => new()
                    {
                        new(BuildingEnums.MainBuilding, 5),
                    },
                BuildingEnums.Palace => new()
                    {
                        new(BuildingEnums.MainBuilding, 5),
                        new(BuildingEnums.Embassy, 1),
                    },
                BuildingEnums.Treasury => new()
                    {
                        new(BuildingEnums.MainBuilding, 10),
                    },
                BuildingEnums.TradeOffice => new()
                    {
                        new(BuildingEnums.Stable, 10),
                        new(BuildingEnums.Marketplace, 20),
                    },
                BuildingEnums.GreatBarracks => new()
                    {
                        new(BuildingEnums.Barracks, 20),
                    },
                BuildingEnums.GreatStable => new()
                    {
                        new(BuildingEnums.Stable, 20),
                    },
                BuildingEnums.StonemasonsLodge => new()
                    {
                        new(BuildingEnums.MainBuilding, 5),
                    },
                BuildingEnums.Brewery => new()
                    {
                        new(BuildingEnums.Granary, 20),
                        new(BuildingEnums.RallyPoint, 10),
                    },
                BuildingEnums.Trapper => new()
                    {
                        new(BuildingEnums.RallyPoint, 1),
                    },
                BuildingEnums.HerosMansion => new()
                    {
                        new(BuildingEnums.MainBuilding, 3),
                        new(BuildingEnums.RallyPoint, 1),
                    },
                BuildingEnums.GreatWarehouse => new()
                    {
                        new(BuildingEnums.MainBuilding, 10),
                    },
                BuildingEnums.GreatGranary => new()
                    {
                        new(BuildingEnums.MainBuilding, 10),
                    },
                BuildingEnums.HorseDrinkingTrough => new()
                    {
                        new(BuildingEnums.RallyPoint, 10),
                        new(BuildingEnums.Stable, 20),
                    },
                //no res/palace
                BuildingEnums.CommandCenter => new()
                    {
                        new(BuildingEnums.MainBuilding, 5),
                    },
                BuildingEnums.Waterworks => new()
                    {
                        new(BuildingEnums.HerosMansion, 10),
                    },
                _ => new(),
            };
        }

        public static bool IsResourceField(this BuildingEnums building)
        {
            int buildingInt = (int)building;
            // If id between 1 and 4, it's resource field
            return buildingInt < 5 && buildingInt > 0;
        }
    }

    public struct PrerequisiteBuilding
    {
        public PrerequisiteBuilding(BuildingEnums type, int level)
        {
            Type = type;
            Level = level;
        }

        public BuildingEnums Type { get; set; }
        public int Level { get; set; }
    }
}