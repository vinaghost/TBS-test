using MainCore.Services;
using ReactiveUI;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.Models.Output;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs
{
    public class DebugViewModel : AccountTabViewModelBase
    {
        private readonly LogSink _logSink;
        private readonly ITaskManager _taskManager;
        private readonly MessageTemplateTextFormatter _formatter;
        public ObservableCollection<TaskItem> Tasks { get; } = new();
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

            buffer.WriteLine(Logs);

            Observable.Start(() => Logs = buffer.ToString(), RxApp.MainThreadScheduler);
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
            Logs = buffer.ToString();
            _isLogLoading = false;
        }
    }
}