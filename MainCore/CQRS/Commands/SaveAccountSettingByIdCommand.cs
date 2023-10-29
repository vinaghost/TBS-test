using MainCore.Common.Enums;
using MainCore.CQRS.Base;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.CQRS.Commands
{
    public class SaveAccountSettingByIdCommand : ByAccountIdRequestBase, IRequest
    {
        public Dictionary<AccountSettingEnums, int> Settings { get; }

        public SaveAccountSettingByIdCommand(AccountId accountId, Dictionary<AccountSettingEnums, int> settings) : base(accountId)
        {
            Settings = settings;
        }
    }

    public class SaveAccountSettingByIdCommandHandler : IRequestHandler<SaveAccountSettingByIdCommand>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public SaveAccountSettingByIdCommandHandler(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task Handle(SaveAccountSettingByIdCommand request, CancellationToken cancellationToken)
        {
            await Task.Run(
                () => SaveSettings(request.AccountId, request.Settings),
                cancellationToken);
        }

        private void SaveSettings(AccountId accountId, Dictionary<AccountSettingEnums, int> settings)
        {
            using var context = _contextFactory.CreateDbContext();

            foreach (var setting in settings)
            {
                context.AccountsSetting
                    .Where(x => x.AccountId == accountId)
                    .Where(x => x.Setting == setting.Key)
                    .ExecuteUpdate(x => x.SetProperty(x => x.Value, setting.Value));
            }
        }
    }
}