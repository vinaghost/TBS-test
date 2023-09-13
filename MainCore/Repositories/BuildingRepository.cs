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
            var buildings = await context.Buildings.Where(x => x.VillageId == villageId).OrderBy(x => x.Id).ToListAsync();
            return buildings;
        }

        public async Task Update(int villageId, List<Building> buildings)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var dbBuildings = await context.Buildings.Where(x => x.VillageId == villageId).OrderBy(x => x.Id).ToListAsync();
                foreach (var building in buildings)
                {
                    var dbBuilding = dbBuildings.FirstOrDefault(x => x.Id == building.Id);
                    if (dbBuilding is null)
                    {
                        await context.AddAsync(building);
                    }
                    else
                    {
                        dbBuilding.Level = building.Level;
                        dbBuilding.Type = building.Type;
                        dbBuilding.IsUnderConstruction = building.IsUnderConstruction;
                        context.Update(building);
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