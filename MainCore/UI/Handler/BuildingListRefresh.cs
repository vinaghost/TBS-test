using MainCore.Common.Notification;
using MainCore.UI.ViewModels.Tabs.Villages;
using MediatR;

namespace MainCore.UI.Handler
{
    internal class BuildingListRefresh : INotificationHandler<BuildingUpdated>
    {
        private readonly BuildViewModel _viewModel;

        public BuildingListRefresh(BuildViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public async Task Handle(BuildingUpdated notification, CancellationToken cancellationToken)
        {
            await _viewModel.BuildingListRefresh(notification.VillageId);
        }
    }
}