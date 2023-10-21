using MainCore.Infrasturecture.Services;
using MainCore.UI.Notification;
using MediatR;

namespace MainCore.UI.Handler
{
    public class UseragentCleaning : INotificationHandler<MainWindowUnloaded>
    {
        private readonly IUseragentManager _useragentManager;

        public UseragentCleaning(IUseragentManager useragentManager)
        {
            _useragentManager = useragentManager;
        }

        public async Task Handle(MainWindowUnloaded notification, CancellationToken cancellationToken)
        {
            await Task.Run(_useragentManager.Dispose, cancellationToken);
        }
    }
}