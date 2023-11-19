﻿using MainCore.Notification.Message;
using MediatR;

namespace MainCore.Notification.Handlers
{
    public class ProxyCacheClean : INotificationHandler<MainWindowUnloaded>
    {
        public async Task Handle(MainWindowUnloaded notification, CancellationToken cancellationToken)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Plugins");
            if (Directory.Exists(path)) await Task.Run(() => Directory.Delete(path, true), cancellationToken);
        }
    }
}