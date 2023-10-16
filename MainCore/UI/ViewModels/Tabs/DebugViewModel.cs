﻿using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.UI.Models.Output;
using MainCore.UI.ViewModels.Abstract;
using ReactiveUI;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Text;

namespace MainCore.UI.ViewModels.Tabs
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class DebugViewModel : AccountTabViewModelBase
    {
        private readonly LogSink _logSink;
        private readonly ITaskManager _taskManager;
        private readonly MessageTemplateTextFormatter _formatter;
        public ObservableCollection<TaskItem> Tasks { get; } = new();
        private string _cacheLog;
        public string _logs;

        public string Logs
        {
            get => _logs;
            set => this.RaiseAndSetIfChanged(ref _logs, value);
        }

        private bool _isLogLoading = false;

        public DebugViewModel(ILogEventSink logSink, ITaskManager taskManager)
        {
            _logSink = logSink as LogSink;
            _logSink.LogEmitted += LogEmitted;
            _taskManager = taskManager;
            _taskManager.TaskUpdated += TaskUpdate;

            _formatter = new("{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
        }

        private void TaskUpdate(int accountId)
        {
            if (!IsActive) return;
            if (accountId != AccountId) return;
            Observable.Start(() => LoadTask(accountId), RxApp.MainThreadScheduler);
        }

        private void LogEmitted(int accountId, LogEvent logEvent)
        {
            if (!IsActive) return;
            if (accountId != AccountId) return;
            if (_isLogLoading) return;
            var buffer = new StringWriter(new StringBuilder());
            _formatter.Format(logEvent, buffer);
            buffer.WriteLine(_cacheLog);
            _cacheLog = buffer.ToString();
            Observable.Start(() => Logs = _cacheLog, RxApp.MainThreadScheduler);
        }

        protected override async Task Load(int accountId)
        {
            await Observable.Start(() =>
            {
                LoadTask(accountId);
                LoadLog(accountId);
            }, RxApp.MainThreadScheduler);
        }

        private void LoadTask(int accountId)
        {
            Tasks.Clear();
            var tasks = _taskManager.GetTaskList(accountId);
            if (tasks.Count == 0) return;

            foreach (var task in tasks)
            {
                Tasks.Add(new(task));
            }
        }

        private void LoadLog(int accountId)
        {
            _isLogLoading = true;
            var logs = _logSink.GetLogs(accountId);
            var buffer = new StringWriter(new StringBuilder());
            foreach (var log in logs)
            {
                _formatter.Format(log, buffer);
            }
            _cacheLog = buffer.ToString();
            Logs = _cacheLog;
            _isLogLoading = false;
        }
    }
}