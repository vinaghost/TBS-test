using MainCore.UI.ViewModels.Tabs.Villages;
using MediatR;

namespace MainCore.Features.Update.Trigger
{
    public class QueueBuildingTrigger : INotification
    {
        public int VillageId { get; set; }

        public QueueBuildingTrigger(int villageId)
        {
            VillageId = villageId;
        }
    }

    public class QueueBuildingTriggerHandler : INotificationHandler<QueueBuildingTrigger>
    {
        private readonly BuildViewModel _buildViewModel;

        public QueueBuildingTriggerHandler(BuildViewModel buildViewModel)
        {
            _buildViewModel = buildViewModel;
        }

        public async Task Handle(QueueBuildingTrigger request, CancellationToken cancellationToken)
        {
            await _buildViewModel.BuildingUpdate(request.VillageId);
        }
    }
}