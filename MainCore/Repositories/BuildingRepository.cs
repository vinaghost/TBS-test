using MainCore.Models;
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
    }
}