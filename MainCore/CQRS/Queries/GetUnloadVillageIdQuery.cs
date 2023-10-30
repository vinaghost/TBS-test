using MainCore.CQRS.Base;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.CQRS.Queries
{
    public class GetUnloadVillageIdQuery : ByAccountIdRequestBase, IRequest<List<VillageId>>
    {
        public GetUnloadVillageIdQuery(AccountId accountId) : base(accountId)
        {
        }
    }

    public class GetUnloadVillageIdQueryHandler : IRequestHandler<GetUnloadVillageIdQuery, List<VillageId>>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public GetUnloadVillageIdQueryHandler(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<VillageId>> Handle(GetUnloadVillageIdQuery request, CancellationToken cancellationToken)
        {
            var villages = await Task.Run(() => GetVillage(request.AccountId), cancellationToken);
            return villages;
        }

        private List<VillageId> GetVillage(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();

            var villages = context.Villages
                .AsNoTracking()
                .Where(x => x.AccountId == accountId)
                .Include(x => x.Buildings)
                .Where(x => x.Buildings.Count < 19)
                .OrderBy(x => x.Name)
                .Select(x => x.Id)
                .ToList();
            return villages;
        }
    }
}