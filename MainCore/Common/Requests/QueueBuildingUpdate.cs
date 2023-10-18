using MainCore.UI.ViewModels.Tabs.Villages;
using MediatR;

namespace MainCore.Common.Requests
{
    public class QueueBuildingUpdate : IRequest
    {
        public int VillageId { get; set; }

        public QueueBuildingUpdate(int villageId)
        {
            VillageId = villageId;
        }
    }

    public class QueueBuildingChangeHandler : IRequestHandler<QueueBuildingUpdate>
    {
        private readonly BuildViewModel _buildViewModel;

        public QueueBuildingChangeHandler(BuildViewModel buildViewModel)
        {
            _buildViewModel = buildViewModel;
        }

        public async Task Handle(QueueBuildingUpdate request, CancellationToken cancellationToken)
        {
            await _buildViewModel.BuildingUpdate(request.VillageId);
        }
    }
}