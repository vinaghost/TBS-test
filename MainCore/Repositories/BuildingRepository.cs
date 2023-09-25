using MainCore.Enums;
using MainCore.Models;
using MainCore.Models.Plans;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Repositories
{
    public class BuildingRepository : IBuildingRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public event Func<int, Task> BuildingUpdated;

        public BuildingRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
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
            var count = await context.QueueBuildings.Where(x => x.VillageId == villageId).CountAsync();
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
                .Where(x => x.VillageId == villageId)
                .Where(x =>
                    x.Type != BuildingEnums.Woodcutter &&
                    x.Type != BuildingEnums.ClayPit &&
                    x.Type != BuildingEnums.IronMine &&
                    x.Type != BuildingEnums.Cropland)
                .CountAsync();
            return count;
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
    }
}