﻿using MainCore.Infrasturecture.AutoRegisterDi;
using Serilog.Core;
using Serilog.Events;

namespace MainCore.Infrasturecture.Services
{
    [RegisterAsSingleton]
    public sealed class LogSink : ILogEventSink
    {
        private Dictionary<int, LinkedList<LogEvent>> Logs { get; } = new();

        public event Action<int, LogEvent> LogEmitted;

        public LinkedList<LogEvent> GetLogs(int accountId)
        {
            var logs = Logs.GetValueOrDefault(accountId);
            if (logs is null)
            {
                logs = new LinkedList<LogEvent>();
                Logs.Add(accountId, logs);
            }
            return logs;
        }

        public void Emit(LogEvent logEvent)
        {
            var logEventPropertyValue = logEvent.Properties.GetValueOrDefault("AccountId");
            if (logEventPropertyValue is null) return;
            if (logEventPropertyValue is not ScalarValue scalarValue) return;
            var accountId = (int)scalarValue.Value;

            var logs = GetLogs(accountId);
            logs.AddFirst(logEvent);
            // keeps 200 message
            while (logs.Count > 200)
            {
                logs.RemoveLast();
            }

            LogEmitted?.Invoke(accountId, logEvent);
        }
    }
}