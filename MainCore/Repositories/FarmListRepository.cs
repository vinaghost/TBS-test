using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Repositories
{
    [RegisterAsTransient]
    public class FarmListRepository : IFarmListRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public FarmListRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public List<FarmListId> GetActiveFarmLists(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var farmListIds = context.FarmLists
                    .Where(x => x.AccountId == accountId.Value)
                    .Where(x => x.IsActive)
                    .Select(x => x.Id)
                    .AsEnumerable()
                    .Select(x => new FarmListId(x))
                    .ToList();
            return farmListIds;
        }
    }
}