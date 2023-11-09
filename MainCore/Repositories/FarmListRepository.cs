using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using MainCore.UI.Models.Output;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

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

        public List<FarmListId> GetActive(AccountId accountId)
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

        public int CountActive(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();

            var count = context.FarmLists
                .AsNoTracking()
                .Where(x => x.AccountId == accountId.Value)
                .Where(x => x.IsActive)
                .Count();
            return count;
        }

        public void ChangeActive(FarmListId farmListId)
        {
            using var context = _contextFactory.CreateDbContext();
            context.FarmLists
               .Where(x => x.Id == farmListId.Value)
               .ExecuteUpdate(x => x.SetProperty(x => x.IsActive, x => !x.IsActive));
        }

        public List<ListBoxItem> GetItems(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();

            var items = context.FarmLists
                .AsNoTracking()
                .Where(x => x.AccountId == accountId.Value)
                .Select(x => new ListBoxItem()
                {
                    Id = x.Id,
                    Color = x.IsActive ? Color.Green : Color.Red,
                    Content = x.Name,
                })
                .ToList();

            return items;
        }
    }
}