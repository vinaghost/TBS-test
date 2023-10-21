using MainCore.Infrasturecture.Persistence;
using MainCore.UI.Notification;
using MediatR;

namespace MainCore.UI.Handler
{
    public class DatabaseInstallation : INotificationHandler<MainWindowLoaded>
    {
        private readonly AppDbContext _context;

        public DatabaseInstallation(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(MainWindowLoaded notification, CancellationToken cancellationToken)
        {
            await _context.Database.EnsureCreatedAsync(cancellationToken);
        }
    }
}