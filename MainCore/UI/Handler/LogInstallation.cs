using MainCore.Infrasturecture.Services;
using MainCore.UI.Notification;
using MediatR;

namespace MainCore.UI.Handler
{
    public class LogInstallation : INotificationHandler<MainWindowLoaded>
    {
        private readonly ILogService _logService;

        public LogInstallation(ILogService logService)
        {
            _logService = logService;
        }

        public async Task Handle(MainWindowLoaded notification, CancellationToken cancellationToken)
        {
            await Task.Run(_logService.Load, cancellationToken);
        }
    }
}