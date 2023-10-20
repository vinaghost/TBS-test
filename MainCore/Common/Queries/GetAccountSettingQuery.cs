using MainCore.Common.Enums;
using MainCore.Infrasturecture.Persistence;
using MediatR;

namespace MainCore.Common.Queries
{
    public class GetAccountSettingQuery : IRequest<int>
    {
        public int AccountId { get; }
        public AccountSettingEnums Setting { get; }

        public GetAccountSettingQuery(int accountId, AccountSettingEnums setting)
        {
            AccountId = accountId;
            Setting = setting;
        }
    }

    public class GetAccountSettingQueryHandler : IRequestHandler<GetAccountSettingQuery, int>
    {
        private readonly AppDbContext _context;

        public GetAccountSettingQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(GetAccountSettingQuery request, CancellationToken cancellationToken)
        {
            var settingEntity = await Task.Run(() =>
                _context.AccountsSetting
                   .Where(x => x.AccountId == request.AccountId)
                   .Where(x => x.Setting == request.Setting)
                   .FirstOrDefault()
            );
            return settingEntity?.Value ?? 0;
        }
    }
}