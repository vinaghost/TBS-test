using MainCore.Common.Enums;
using MainCore.CQRS.Base;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.CQRS.Commands
{
    public class SaveVillageSettingByIdCommand : ByVillageIdRequestBase, IRequest
    {
        public Dictionary<VillageSettingEnums, int> Settings { get; }

        public SaveVillageSettingByIdCommand(VillageId VillageId, Dictionary<VillageSettingEnums, int> settings) : base(VillageId)
        {
            Settings = settings;
        }
    }

    public class SaveVillageSettingByIdCommandHandler : IRequestHandler<SaveVillageSettingByIdCommand>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public SaveVillageSettingByIdCommandHandler(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task Handle(SaveVillageSettingByIdCommand request, CancellationToken cancellationToken)
        {
            await Task.Run(
                () => SaveSettings(request.VillageId, request.Settings),
                cancellationToken);
        }

        private void SaveSettings(VillageId VillageId, Dictionary<VillageSettingEnums, int> settings)
        {
            using var context = _contextFactory.CreateDbContext();

            foreach (var setting in settings)
            {
                context.VillagesSetting
                    .Where(x => x.VillageId == VillageId)
                    .Where(x => x.Setting == setting.Key)
                    .ExecuteUpdate(x => x.SetProperty(x => x.Value, setting.Value));
            }
        }
    }
}