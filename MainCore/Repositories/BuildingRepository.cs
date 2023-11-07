using MainCore.Common.Enums;
using MainCore.Common.Models;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MainCore.Repositories
{
    [RegisterAsTransient]
    public class BuildingRepository : IBuildingRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public BuildingRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public BuildingDto GetBuilding(VillageId villageId, int location)
        {
            using var context = _contextFactory.CreateDbContext();
            var building = context.Buildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Location == location)
                .ToDto()
                .FirstOrDefault();
            return building;
        }

        public int CountQueueBuilding(VillageId villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var count = context.QueueBuildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Type != BuildingEnums.Site)
                .Count();
            return count;
        }

        public int CountResourceQueueBuilding(VillageId villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var resourceTypes = new List<BuildingEnums>()
            {
                BuildingEnums.Woodcutter,
                BuildingEnums.ClayPit,
                BuildingEnums.IronMine,
                BuildingEnums.Cropland
            };

            var count = context.QueueBuildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => resourceTypes.Contains(x.Type))
                .Count();
            return count;
        }

        public bool IsRallyPointExists(VillageId villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var isExists = context.Buildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Type == BuildingEnums.RallyPoint)
                .Where(x => x.Level > 0)
                .Any();
            return isExists;
        }

        public bool IsEmptySite(VillageId villageId, int location)
        {
            using var context = _contextFactory.CreateDbContext();
            var isEmptySite = context.Buildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Type == BuildingEnums.Site)
                .Where(x => x.Location == location)
                .Any();
            return isEmptySite;
        }

        public bool IsJobComplete(VillageId villageId, JobDto job)
        {
            if (job.Type == JobTypeEnums.ResourceBuild) return false;
            var plan = JsonSerializer.Deserialize<NormalBuildPlan>(job.Content);

            using var context = _contextFactory.CreateDbContext();

            var queueBuilding = context.QueueBuildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Location == plan.Location)
                .OrderByDescending(x => x.Level)
                .FirstOrDefault();

            if (queueBuilding is not null && queueBuilding.Level >= plan.Level) return true;

            var villageBuilding = context.Buildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Location == plan.Location)
                .FirstOrDefault();
            if (villageBuilding is not null && villageBuilding.Level >= plan.Level) return true;

            return false;
        }

        public Building GetCropland(VillageId villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var building = context.Buildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Type == BuildingEnums.Cropland)
                .OrderBy(x => x.Level)
                .FirstOrDefault();
            return building;
        }

        public NormalBuildPlan GetNormalBuildPlan(VillageId villageId, ResourceBuildPlan plan)
        {
            using var context = _contextFactory.CreateDbContext();

            var resourceTypes = new List<BuildingEnums>();

            switch (plan.Plan)
            {
                case ResourcePlanEnums.AllResources:
                    resourceTypes.AddRange(new[]
                    {
                         BuildingEnums.Woodcutter,
                         BuildingEnums.ClayPit,
                         BuildingEnums.IronMine,
                         BuildingEnums.Cropland,
                    });
                    break;

                case ResourcePlanEnums.ExcludeCrop:
                    resourceTypes.AddRange(new[]
                    {
                         BuildingEnums.Woodcutter,
                         BuildingEnums.ClayPit,
                         BuildingEnums.IronMine,
                    });
                    break;

                case ResourcePlanEnums.OnlyCrop:
                    resourceTypes.AddRange(new[]
                    {
                         BuildingEnums.Cropland,
                    });
                    break;

                default:
                    break;
            }
            var buildings = context.Buildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => resourceTypes.Contains(x.Type))
                .Where(x => x.Level < plan.Level)
                .ToList();

            var queueBuildings = context.QueueBuildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => resourceTypes.Contains(x.Type))
                .GroupBy(x => x.Location)
                .Select(x => new
                {
                    Location = x.Key,
                    UpgradingLevel = x.OrderBy(x => x.Level).Count()
                })
                .AsEnumerable();

            foreach (var queueBuilding in queueBuildings)
            {
                var building = buildings.FirstOrDefault(x => x.Location == queueBuilding.Location);
                building.Level += queueBuilding.UpgradingLevel;
            }

            buildings = buildings
                .Where(x => x.Level < plan.Level)
                .ToList();
            if (!buildings.Any()) return null;
            var chosenOne = buildings
                .OrderBy(x => x.Level)
                .FirstOrDefault();

            var normalBuildPlan = new NormalBuildPlan()
            {
                Type = chosenOne.Type,
                Level = chosenOne.Level + 1,
                Location = chosenOne.Location,
            };
            return normalBuildPlan;
        }
    }
}