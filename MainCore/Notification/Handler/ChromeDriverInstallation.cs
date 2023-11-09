using MainCore.Infrasturecture.Services;
using MainCore.UI.Notification;
using MainCore.UI.ViewModels.UserControls;
using MediatR;

namespace MainCore.Notification.Handler
{
    public class ChromeDriverInstallation : INotificationHandler<MainWindowLoaded>
    {
        private readonly IChromeDriverInstaller _chromeDriverInstaller;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        public ChromeDriverInstallation(IChromeDriverInstaller chromeDriverInstaller, WaitingOverlayViewModel waitingOverlayViewModel)
        {
            _chromeDriverInstaller = chromeDriverInstaller;
            _waitingOverlayViewModel = waitingOverlayViewModel;
        }

        public async Task Handle(MainWindowLoaded notification, CancellationToken cancellationToken)
        {
            await _waitingOverlayViewModel.ChangeMessage("installing chrome driver");
            await Task.Run(_chromeDriverInstaller.Install, cancellationToken);
        }
    }
}