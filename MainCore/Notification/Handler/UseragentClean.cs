using MainCore.Services;
using MainCore.UI.Notification;
using MediatR;

namespace MainCore.Notification.Handler
{
    public class UseragentClean : INotificationHandler<MainWindowUnloaded>
    {
        private readonly IUseragentManager _useragentManager;

        public UseragentClean(IUseragentManager useragentManager)
        {
            _useragentManager = useragentManager;
        }

        public async Task Handle(MainWindowUnloaded notification, CancellationToken cancellationToken)
        {
            await Task.Run(_useragentManager.Dispose, cancellationToken);
        }
    }
}