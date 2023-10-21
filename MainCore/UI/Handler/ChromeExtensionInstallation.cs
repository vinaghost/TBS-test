using MainCore.Infrasturecture.Services;
using MainCore.UI.Notification;
using MediatR;

namespace MainCore.UI.Handler
{
    public class ChromeExtensionInstallation : INotificationHandler<MainWindowLoaded>
    {
        private readonly IChromeManager _chromeManager;

        public ChromeExtensionInstallation(IChromeManager chromeManager)
        {
            _chromeManager = chromeManager;
        }

        public async Task Handle(MainWindowLoaded notification, CancellationToken cancellationToken)
        {
            await Task.Run(_chromeManager.LoadExtension, cancellationToken);
        }
    }
}