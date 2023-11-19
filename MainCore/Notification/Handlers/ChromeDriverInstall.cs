﻿using MainCore.Notification.Message;
using MainCore.Services;
using MainCore.UI.ViewModels.UserControls;
using MediatR;

namespace MainCore.Notification.Handlers
{
    public class ChromeDriverInstall : INotificationHandler<MainWindowLoaded>
    {
        private readonly IChromeDriverInstaller _chromeDriverInstaller;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        public ChromeDriverInstall(IChromeDriverInstaller chromeDriverInstaller, WaitingOverlayViewModel waitingOverlayViewModel)
        {
            _chromeDriverInstaller = chromeDriverInstaller;
            _waitingOverlayViewModel = waitingOverlayViewModel;
        }

        public async Task Handle(MainWindowLoaded notification, CancellationToken cancellationToken)
        {
            await _waitingOverlayViewModel.ChangeMessage("installing chrome driver");
            await Task.Run(_chromeDriverInstaller.Install, cancellationToken);
        }
    }
}