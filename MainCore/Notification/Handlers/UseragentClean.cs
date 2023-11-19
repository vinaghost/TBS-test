using MainCore.Notification.Message;
using MainCore.Services;
using MediatR;

namespace MainCore.Notification.Handlers
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