using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Common.Repositories
{
    [RegisterAsSingleton]
    public class QueueBuildingRepository : IQueueBuildingRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public event Func<int, Task> QueueBuildingUpdated;

        public QueueBuildingRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<QueueBuilding> GetFirst(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var queueBuildings = context.QueueBuildings
                .Where(x => x.VillageId == villageId && x.Type != BuildingEnums.Site)
                .OrderBy(x => x.Position);
            var queueBuilding = await queueBuildings.FirstOrDefaultAsync();
            return queueBuilding;
        }

        public async Task<List<QueueBuilding>> GetList(int villageId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var queueBuildings = context.QueueBuildings
                .Where(x => x.VillageId == villageId && x.Type != BuildingEnums.Site)
                .OrderBy(x => x.Position);
            return await queueBuildings.ToListAsync();
        }

        public async Task Update(int villageId, List<Building> buildings)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var queueBuildings = context.QueueBuildings
                    .Where(x => x.VillageId == villageId && x.Type != BuildingEnums.Site);

                if (buildings.Count == 1)
                {
                    var building = buildings[0];
                    queueBuildings = queueBuildings.Where(x => x.Type == building.Type);
                    var list = await queueBuildings.ToListAsync();
                    foreach (var item in list)
                    {
                        item.Location = building.Location;
                    }
                    context.UpdateRange(list);
                }
                else if (buildings.Count == 2)
                {
                    foreach (var building in buildings)
                    {
                        var queueBuilding = await queueBuildings.FirstOrDefaultAsync(x => x.Type == building.Type);
                        queueBuilding.Location = building.Location;
                        context.Update(queueBuilding);
                    }
                }
                await context.SaveChangesAsync();
            }
            if (QueueBuildingUpdated is not null)
            {
                await QueueBuildingUpdated(villageId);
            }
        }

        public async Task Update(int villageId, List<QueueBuilding> queueBuildings)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var dbQueueBuildings = context.QueueBuildings.Where(x => x.VillageId == villageId);

                for (var i = 0; i < queueBuildings.Count; i++)
                {
                    var queueBuilding = queueBuildings[i];
                    var dbQueueBuilding = await dbQueueBuildings.FirstOrDefaultAsync(x => x.Position == i);
                    if (dbQueueBuilding is null)
                    {
                        await context.AddAsync(queueBuilding);
                    }
                    else
                    {
                        dbQueueBuilding.Type = queueBuilding.Type;
                        dbQueueBuilding.Level = queueBuilding.Level;
                        dbQueueBuilding.CompleteTime = queueBuilding.CompleteTime;
                        context.Update(dbQueueBuilding);
                    }
                }
                await context.SaveChangesAsync();
            }
            if (QueueBuildingUpdated is not null)
            {
                await QueueBuildingUpdated(villageId);
            }
        }
    }
}