using MainCore.UI.Notification;
using MediatR;

namespace MainCore.UI.Handler
{
    public class ProxyCacheCleaning : INotificationHandler<MainWindowUnloaded>
    {
        public async Task Handle(MainWindowUnloaded notification, CancellationToken cancellationToken)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Plugins");
            if (Directory.Exists(path)) await Task.Run(() => Directory.Delete(path, true), cancellationToken);
        }
    }
}