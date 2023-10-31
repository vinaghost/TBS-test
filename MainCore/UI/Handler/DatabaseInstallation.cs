using MainCore.Infrasturecture.Persistence;
using MainCore.UI.Notification;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.UI.Handler
{
    public class DatabaseInstallation : INotificationHandler<MainWindowLoaded>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public DatabaseInstallation(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task Handle(MainWindowLoaded notification, CancellationToken cancellationToken)
        {
            using var context = _contextFactory.CreateDbContext();
            await context.Database.EnsureCreatedAsync(cancellationToken);
        }
    }
}