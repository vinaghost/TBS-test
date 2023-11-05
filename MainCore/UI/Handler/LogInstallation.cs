using MainCore.Infrasturecture.Services;
using MainCore.UI.Notification;
using MainCore.UI.ViewModels.UserControls;
using MediatR;

namespace MainCore.UI.Handler
{
    public class LogInstallation : INotificationHandler<MainWindowLoaded>
    {
        private readonly ILogService _logService;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        public LogInstallation(ILogService logService, WaitingOverlayViewModel waitingOverlayViewModel)
        {
            _logService = logService;
            _waitingOverlayViewModel = waitingOverlayViewModel;
        }

        public async Task Handle(MainWindowLoaded notification, CancellationToken cancellationToken)
        {
            await _waitingOverlayViewModel.ChangeMessage("installing log system");
            await Task.Run(_logService.Load, cancellationToken);
        }
    }
}