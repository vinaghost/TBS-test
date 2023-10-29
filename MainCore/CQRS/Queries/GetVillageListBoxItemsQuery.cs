using MainCore.CQRS.Base;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MainCore.UI.Models.Output;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.CQRS.Queries
{
    public class GetVillageListBoxItemsQuery : ByAccountIdRequestBase, IRequest<List<ListBoxItem>>
    {
        public GetVillageListBoxItemsQuery(AccountId accountId) : base(accountId)
        {
        }
    }

    public class GetVillageListBoxItemsQueryHandler : IRequestHandler<GetVillageListBoxItemsQuery, List<ListBoxItem>>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public GetVillageListBoxItemsQueryHandler(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<ListBoxItem>> Handle(GetVillageListBoxItemsQuery request, CancellationToken cancellationToken)
        {
            var villages = await Task.Run(() => GetVillage(request.AccountId), cancellationToken);
            return villages;
        }

        private List<ListBoxItem> GetVillage(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();

            var villages = context.Villages
                    .Where(x => x.AccountId == accountId)
                    .OrderBy(x => x.Name)
                    .Select(x => new ListBoxItem()
                    {
                        Id = x.Id.Value,
                        Content = $"{x.Name}{Environment.NewLine}({x.X}|{x.Y})",
                    })
                    .ToList();
            return villages;
        }
    }
}