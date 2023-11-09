using MainCore.Infrasturecture.Services;
using MainCore.UI.Notification;
using MediatR;

namespace MainCore.Notification.Handler
{
    public class ChromeCleaning : INotificationHandler<MainWindowUnloaded>
    {
        private readonly IChromeManager _chromeManager;

        public ChromeCleaning(IChromeManager chromeManager)
        {
            _chromeManager = chromeManager;
        }

        public async Task Handle(MainWindowUnloaded notification, CancellationToken cancellationToken)
        {
            await Task.Run(_chromeManager.Shutdown, cancellationToken);
        }
    }
}