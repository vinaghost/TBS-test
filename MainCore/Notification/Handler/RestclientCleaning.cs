using MainCore.Infrasturecture.Services;
using MainCore.UI.Notification;
using MediatR;

namespace MainCore.Notification.Handler
{
    public class RestclientCleaningpublic : INotificationHandler<MainWindowUnloaded>
    {
        private readonly IRestClientManager _restClientManager;

        public RestclientCleaningpublic(IRestClientManager restClientManager)
        {
            _restClientManager = restClientManager;
        }

        public async Task Handle(MainWindowUnloaded notification, CancellationToken cancellationToken)
        {
            await Task.Run(_restClientManager.Shutdown, cancellationToken);
        }
    }
}