﻿using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace MainCore.Services
{
    public class LogService : ILogService
    {
        private readonly Dictionary<int, ILogger> _loggers = new();

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly LogSink _logSink;

        public LogService(IDbContextFactory<AppDbContext> contextFactory, IServiceProvider serviceProvider, ILogEventSink logSink)
        {
            _contextFactory = contextFactory;
            _serviceProvider = serviceProvider;
            _logSink = logSink as LogSink;

            _logSink.LogEmitted += OnLogEmitted;
        }

        public void Init()
        {
            Log.Logger = new LoggerConfiguration()
              .ReadFrom.Services(_serviceProvider)
              .WriteTo.Map("Account", "Other", (acc, wt) =>
                    wt.File($"./logs/log-{acc}-.txt",
                            rollingInterval: RollingInterval.Day,
                            outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"))
              .CreateLogger();
        }

        public void Shutdown()
        {
            Log.CloseAndFlush();
        }

        public LinkedList<LogEvent> GetLog(int accountId)
        {
            var logs = _logSink.GetLogs(accountId); ;
            return logs;
        }

        private void OnLogEmitted(LogEvent logEvent)
        {
            _ = 2;
        }

        public ILogger GetLogger(int accountId)
        {
            var logger = _loggers.GetValueOrDefault(accountId);
            if (logger is null)
            {
                using var context = _contextFactory.CreateDbContext();
                var account = context.Accounts.Find(accountId);

                var uri = new Uri(account.Server);
                logger = Log.ForContext("Account", $"{account.Username}_{uri.Host}")
                            .ForContext("AccountId", accountId);
                _loggers.Add(accountId, logger);
            }
            return logger;
        }
    }
}