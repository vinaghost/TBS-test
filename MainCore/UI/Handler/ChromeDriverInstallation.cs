using MainCore.Infrasturecture.Services;
using MainCore.UI.Notification;
using MediatR;

namespace MainCore.UI.Handler
{
    public class ChromeDriverInstallation : INotificationHandler<MainWindowLoaded>
    {
        private readonly IChromeDriverInstaller _chromeDriverInstaller;

        public ChromeDriverInstallation(IChromeDriverInstaller chromeDriverInstaller)
        {
            _chromeDriverInstaller = chromeDriverInstaller;
        }

        public async Task Handle(MainWindowLoaded notification, CancellationToken cancellationToken)
        {
            await Task.Run(_chromeDriverInstaller.Install, cancellationToken);
        }
    }
}