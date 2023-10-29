using MainCore.Common.Extensions;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using MainCore.UI.Models.Output;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.CQRS.Queries
{
    public class GetAccountListBoxItemsQuery : IRequest<List<ListBoxItem>>
    {
    }

    public class GetAccountItemsQueryHandler : IRequestHandler<List<ListBoxItem>>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ITaskManager _taskManager;

        public GetAccountItemsQueryHandler(IDbContextFactory<AppDbContext> contextFactory, ITaskManager taskManager)
        {
            _contextFactory = contextFactory;
            _taskManager = taskManager;
        }

        public async Task<List<ListBoxItem>> Handle(GetAccountListBoxItemsQuery _, CancellationToken cancellationToken)
        {
            var items = await Task.Run(() => GetItems(), cancellationToken);
            return items;
        }

        private List<ListBoxItem> GetItems()
        {
            using var context = _contextFactory.CreateDbContext();

            var accounts = context.Accounts
                .AsEnumerable();
            var items = accounts.Select(x =>
            {
                var serverUrl = new Uri(x.Server);
                var status = _taskManager.GetStatus(x.Id);
                return new ListBoxItem()
                {
                    Id = x.Id.Value,
                    Color = status.GetColor(),
                    Content = $"{x.Username}{Environment.NewLine}({serverUrl.Host})"
                };
            }).ToList();

            return items;
        }
    }
}