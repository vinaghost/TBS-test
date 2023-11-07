using MainCore.Common.Enums;
using MainCore.CQRS.Base;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.CQRS.Queries
{
    public class GetVillageSettingDictonaryByIdQuery : ByVillageIdRequestBase, IRequest<Dictionary<VillageSettingEnums, int>>
    {
        public GetVillageSettingDictonaryByIdQuery(VillageId VillageId) : base(VillageId)
        {
        }
    }

    public class GetVillageSettingDictonaryByIdQueryHandler : IRequestHandler<GetVillageSettingDictonaryByIdQuery, Dictionary<VillageSettingEnums, int>>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public GetVillageSettingDictonaryByIdQueryHandler(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Dictionary<VillageSettingEnums, int>> Handle(GetVillageSettingDictonaryByIdQuery request, CancellationToken cancellationToken)
        {
            var settings = await Task.Run(() => Get(request.VillageId), cancellationToken);
            return settings;
        }

        private Dictionary<VillageSettingEnums, int> Get(VillageId villageId)
        {
            using var context = _contextFactory.CreateDbContext();

            var settings = context.VillagesSetting
                .AsNoTracking()
                .Where(x => x.VillageId == villageId.Value)
                .ToDictionary(x => x.Setting, x => x.Value);
            return settings;
        }
    }
}