using MainCore.Infrasturecture.Services;
using MainCore.UI.Notification;
using MainCore.UI.ViewModels.UserControls;
using MediatR;

namespace MainCore.UI.Handler
{
    public class ChromeUserAgentInstallation : INotificationHandler<MainWindowLoaded>
    {
        private readonly IUseragentManager _useragentManager;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        public ChromeUserAgentInstallation(IUseragentManager useragentManager, WaitingOverlayViewModel waitingOverlayViewModel)
        {
            _useragentManager = useragentManager;
            _waitingOverlayViewModel = waitingOverlayViewModel;
        }

        public async Task Handle(MainWindowLoaded notification, CancellationToken cancellationToken)
        {
            await _waitingOverlayViewModel.ChangeMessage("loading chrome useragent");
            await Task.Run(_useragentManager.Load, cancellationToken);
        }
    }
}