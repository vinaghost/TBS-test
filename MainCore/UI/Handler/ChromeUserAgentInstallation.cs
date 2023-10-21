using MainCore.Infrasturecture.Services;
using MainCore.UI.Notification;
using MediatR;

namespace MainCore.UI.Handler
{
    public class ChromeUserAgentInstallation : INotificationHandler<MainWindowLoaded>
    {
        private readonly IUseragentManager _useragentManager;

        public ChromeUserAgentInstallation(IUseragentManager useragentManager)
        {
            _useragentManager = useragentManager;
        }

        public async Task Handle(MainWindowLoaded notification, CancellationToken cancellationToken)
        {
            await Task.Run(_useragentManager.Load, cancellationToken);
        }
    }
}