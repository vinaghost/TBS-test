using MainCore.Infrasturecture.Persistence;
using MainCore.UI.Notification;
using MainCore.UI.ViewModels.UserControls;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Notification.Handler
{
    public class DatabaseInstallation : INotificationHandler<MainWindowLoaded>
    {
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public DatabaseInstallation(IDbContextFactory<AppDbContext> contextFactory, WaitingOverlayViewModel waitingOverlayViewModel)
        {
            _contextFactory = contextFactory;
            _waitingOverlayViewModel = waitingOverlayViewModel;
        }

        public async Task Handle(MainWindowLoaded notification, CancellationToken cancellationToken)
        {
            await _waitingOverlayViewModel.ChangeMessage("loading database");
            using var context = _contextFactory.CreateDbContext();
            var created = await context.Database.EnsureCreatedAsync(cancellationToken);
            if (created)
            {
                await Task.Run(context.FillAccountSettings, cancellationToken);
                await Task.Run(context.FillVillageSettings, cancellationToken);
            }
        }
    }
}