using MainCore.Services;
using MainCore.UI.Notification;
using MediatR;

namespace MainCore.Notification.Handler
{
    public class RestclientClean : INotificationHandler<MainWindowUnloaded>
    {
        private readonly IRestClientManager _restClientManager;

        public RestclientClean(IRestClientManager restClientManager)
        {
            _restClientManager = restClientManager;
        }

        public async Task Handle(MainWindowUnloaded notification, CancellationToken cancellationToken)
        {
            await Task.Run(_restClientManager.Shutdown, cancellationToken);
        }
    }
}