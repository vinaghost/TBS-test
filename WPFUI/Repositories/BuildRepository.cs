using MainCore;
using MainCore.Enums;
using MainCore.Models.Plans;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

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
                .Where(x => x.VillageId == villageId)
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

            return buildings;
        }

        private async Task<Dictionary<ResourcePlanEnums, int>> GetResourcePlans(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

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
            return resourceJobs;
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
            switch (building)
            {
                case BuildingEnums.EarthWall:
                case BuildingEnums.CityWall:
                case BuildingEnums.Palisade:
                case BuildingEnums.StoneWall:
                case BuildingEnums.MakeshiftWall:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsMultipleBuilding(this BuildingEnums building)
        {
            switch (building)
            {
                case BuildingEnums.Warehouse:
                case BuildingEnums.Granary:
                case BuildingEnums.GreatWarehouse:
                case BuildingEnums.GreatGranary:
                case BuildingEnums.Trapper:
                case BuildingEnums.Cranny:
                    return true;

                default:
                    return false;
            }
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

        public static bool IsResourceField(this BuildingEnums building)
        {
            int buildingInt = (int)building;
            // If id between 1 and 4, it's resource field
            return buildingInt < 5 && buildingInt > 0;
        }
    }
}