using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace MainCore.Infrasturecture.Services
{
    [RegisterAsSingleton]
    public sealed class LogService : ILogService
    {
        private readonly Dictionary<int, ILogger> _loggers = new();

        private readonly AppDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly LogSink _logSink;

        public LogService(AppDbContext context, IServiceProvider serviceProvider, ILogEventSink logSink)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            _logSink = logSink as LogSink;
        }

        public void Init()
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

        public LinkedList<LogEvent> GetLog(int accountId)
        {
            var logs = _logSink.GetLogs(accountId);
            return logs;
        }

        public ILogger GetLogger(int accountId)
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