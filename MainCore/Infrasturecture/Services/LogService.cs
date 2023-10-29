using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace MainCore.Infrasturecture.Services
{
    [RegisterAsSingleton]
    public sealed class LogService : ILogService
    {
        private readonly Dictionary<AccountId, ILogger> _loggers = new();

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly LogSink _logSink;

        public LogService(IDbContextFactory<AppDbContext> contextFactory, IServiceProvider serviceProvider, ILogEventSink logSink)
        {
            _contextFactory = contextFactory;
            _serviceProvider = serviceProvider;
            _logSink = logSink as LogSink;
        }

        public void Load()
        {
            Log.Logger = new LoggerConfiguration()
              .ReadFrom.Services(_serviceProvider)
              .WriteTo.Map("Account", "Other", (acc, wt) =>
                    wt.File($"./logs/log-{acc}-.txt",
                            rollingInterval: RollingInterval.Day,
                            outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Line:lj}{NewLine}{Exception}"))
              .CreateLogger();
        }

        public void Shutdown()
        {
            Log.CloseAndFlush();
        }

        public LinkedList<LogEvent> GetLog(AccountId accountId)
        {
            var logs = _logSink.GetLogs(accountId);
            return logs;
        }

        public ILogger GetLogger(AccountId accountId)
        {
            var logger = _loggers.GetValueOrDefault(accountId);
            if (logger is null)
            {
                var account = _context.Accounts.Find(accountId);

                var uri = new Uri(account.Server);
                logger = Log.ForContext("Account", $"{account.Username}_{uri.Host}")
                            .ForContext("VillageId", accountId);
                _loggers.Add(accountId, logger);
            }
            return logger;
        }
    }
}