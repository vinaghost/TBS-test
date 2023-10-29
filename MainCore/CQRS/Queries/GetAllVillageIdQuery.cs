using MainCore.CQRS.Base;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.CQRS.Queries
{
    public class GetAllVillageIdQuery : ByAccountIdRequestBase, IRequest<List<VillageId>>
    {
        public GetAllVillageIdQuery(AccountId accountId) : base(accountId)
        {
        }
    }

    public class GetAllVillageIdQueryHandler : IRequestHandler<GetAllVillageIdQuery, List<VillageId>>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public GetAllVillageIdQueryHandler(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<VillageId>> Handle(GetAllVillageIdQuery request, CancellationToken cancellationToken)
        {
            var villages = await Task.Run(() => GetVillage(request.AccountId), cancellationToken);
            return villages;
        }

        private List<VillageId> GetVillage(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();

            var villages = context.Villages
                    .Where(x => x.AccountId == accountId)
                    .Select(x => x.Id)
                    .ToList();
            return villages;
        }
    }
}