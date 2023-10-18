using MainCore.UI.ViewModels.Tabs.Villages;
using MediatR;

namespace MainCore.Common.Requests
{
    public class BuildingUpdate : IRequest
    {
        public int VillageId { get; set; }

        public BuildingUpdate(int villageId)
        {
            VillageId = villageId;
        }
    }

    public class BuildingChangeHandler : IRequestHandler<BuildingUpdate>
    {
        private readonly BuildViewModel _buildViewModel;

        public BuildingChangeHandler(BuildViewModel buildViewModel)
        {
            _buildViewModel = buildViewModel;
        }

        public async Task Handle(BuildingUpdate request, CancellationToken cancellationToken)
        {
            await _buildViewModel.BuildingUpdate(request.VillageId);
        }
    }
}