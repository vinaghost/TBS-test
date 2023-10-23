using MainCore.Common.Notification;
using MainCore.UI.ViewModels.UserControls;
using MediatR;

namespace MainCore.UI.Handler
{
    public class StatusRefresh : INotificationHandler<StatusUpdated>
    {
        private readonly MainLayoutViewModel _mainlayoutViewModel;

        public StatusRefresh(MainLayoutViewModel mainlayoutViewModel)
        {
            _mainlayoutViewModel = mainlayoutViewModel;
        }

        public async Task Handle(StatusUpdated notification, CancellationToken cancellationToken)
        {
            await _mainlayoutViewModel.LoadStatus(notification.AccountId, notification.Status);
        }
    }
}