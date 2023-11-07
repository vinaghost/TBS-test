using MainCore.CQRS.Base;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MainCore.UI.Models.Output;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace MainCore.CQRS.Queries
{
    public class GetFarmListListBoxItemsQuery : ByAccountIdRequestBase, IRequest<List<ListBoxItem>>
    {
        public GetFarmListListBoxItemsQuery(AccountId accountId) : base(accountId)
        {
        }
    }

    public class GetFarmListListBoxItemsQueryHandler : IRequestHandler<GetFarmListListBoxItemsQuery, List<ListBoxItem>>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public GetFarmListListBoxItemsQueryHandler(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<ListBoxItem>> Handle(GetFarmListListBoxItemsQuery request, CancellationToken cancellationToken)
        {
            var items = await Task.Run(() => GetItems(request.AccountId), cancellationToken);
            return items;
        }

        private List<ListBoxItem> GetItems(AccountId accountId)
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