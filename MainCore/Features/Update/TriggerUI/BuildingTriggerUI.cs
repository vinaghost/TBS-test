using MainCore.UI.ViewModels.Tabs.Villages;
using MediatR;

namespace MainCore.Features.Update.TriggerUI
{
    public class BuildingTriggerUI : IRequest
    {
        public int VillageId { get; set; }

        public BuildingTriggerUI(int villageId)
        {
            VillageId = villageId;
        }
    }

    public class BuildingTriggerUIHandler : IRequestHandler<BuildingTriggerUI>
    {
        private readonly BuildViewModel _buildViewModel;

        public BuildingTriggerUIHandler(BuildViewModel buildViewModel)
        {
            _buildViewModel = buildViewModel;
        }

        public async Task Handle(BuildingTriggerUI request, CancellationToken cancellationToken)
        {
            await _buildViewModel.BuildingUpdate(request.VillageId);
        }
    }
}