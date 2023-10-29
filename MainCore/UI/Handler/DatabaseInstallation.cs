using MainCore.Infrasturecture.Persistence;
using MainCore.UI.Notification;
using MediatR;

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
            await _context.Database.EnsureCreatedAsync(cancellationToken);
        }
    }
}