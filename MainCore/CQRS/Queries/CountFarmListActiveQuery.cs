using MainCore.CQRS.Base;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.CQRS.Queries
{
    public class CountFarmListActiveQuery : ByAccountIdRequestBase, IRequest<int>
    {
        public CountFarmListActiveQuery(AccountId accountId) : base(accountId)
        {
        }
    }

    public class CountFarmListActiveQueryHandler : IRequestHandler<CountFarmListActiveQuery, int>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public CountFarmListActiveQueryHandler(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<int> Handle(CountFarmListActiveQuery request, CancellationToken cancellationToken)
        {
            var count = await Task.Run(() => CountActive(request.AccountId), cancellationToken);
            return count;
        }

        public int CountActive(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();

            var count = context.FarmLists
                .AsNoTracking()
                .Where(x => x.AccountId == accountId)
                .Where(x => x.IsActive)
                .Count();
            return count;
        }
    }
}