using MainCore.Common.Enums;
using MainCore.CQRS.Base;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.CQRS.Queries
{
    public class GetAccountSettingDictonaryByIdQuery : ByAccountIdRequestBase, IRequest<Dictionary<AccountSettingEnums, int>>
    {
        public GetAccountSettingDictonaryByIdQuery(AccountId accountId) : base(accountId)
        {
        }
    }

    public class GetAccountSettingDictonaryByIdQueryHandler : IRequestHandler<GetAccountSettingDictonaryByIdQuery, Dictionary<AccountSettingEnums, int>>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public GetAccountSettingDictonaryByIdQueryHandler(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Dictionary<AccountSettingEnums, int>> Handle(GetAccountSettingDictonaryByIdQuery request, CancellationToken cancellationToken)
        {
            var settings = await Task.Run(() => Get(request.AccountId), cancellationToken);
            return settings;
        }

        private Dictionary<AccountSettingEnums, int> Get(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();

            var settings = context.AccountsSetting
                .Where(x => x.AccountId == accountId)
                .ToDictionary(x => x.Setting, x => x.Value);
            return settings;
        }
    }
}