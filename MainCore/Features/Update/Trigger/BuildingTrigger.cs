using MainCore.UI.ViewModels.Tabs.Villages;
using MediatR;

namespace MainCore.Features.Update.Trigger
{
    public class BuildingTrigger : INotification
    {
        public int VillageId { get; set; }

        public BuildingTrigger(int villageId)
        {
            VillageId = villageId;
        }
    }

    public class BuildingTriggerHandler : INotificationHandler<BuildingTrigger>
    {
        private readonly BuildViewModel _buildViewModel;

        public BuildingTriggerHandler(BuildViewModel buildViewModel)
        {
            _buildViewModel = buildViewModel;
        }

        public async Task Handle(BuildingTrigger request, CancellationToken cancellationToken)
        {
            await _buildViewModel.BuildingUpdate(request.VillageId);
        }
    }
}