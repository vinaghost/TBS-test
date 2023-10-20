using MainCore.UI.ViewModels.Tabs.Villages;
using MediatR;

namespace MainCore.Features.Update.TriggerUI
{
    public class QueueBuildingTriggerUI : IRequest
    {
        public int VillageId { get; set; }

        public QueueBuildingTriggerUI(int villageId)
        {
            VillageId = villageId;
        }
    }

    public class QueueBuildingTriggerUIHandler : IRequestHandler<QueueBuildingTriggerUI>
    {
        private readonly BuildViewModel _buildViewModel;

        public QueueBuildingTriggerUIHandler(BuildViewModel buildViewModel)
        {
            _buildViewModel = buildViewModel;
        }

        public async Task Handle(QueueBuildingTriggerUI request, CancellationToken cancellationToken)
        {
            await _buildViewModel.BuildingUpdate(request.VillageId);
        }
    }
}