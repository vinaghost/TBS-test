using MainCore.Common.Enums;
using MainCore.Common.Tasks;
using MainCore.Infrasturecture.AutoRegisterDi;
using Microsoft.Extensions.DependencyInjection;
using Splat;

namespace MainCore.Infrasturecture.Services
{
    [RegisterAsSingleton]
    public sealed class TaskManager : ITaskManager
    {
        public class TaskInfo
        {
            public bool IsExecuting { get; set; } = false;
            public StatusEnums Status { get; set; } = StatusEnums.Offline;
            public CancellationTokenSource CancellationTokenSource { get; set; } = null;

            public List<TaskBase> TaskList { get; set; } = new();
        }

        private readonly Dictionary<int, TaskInfo> _tasks = new();

        public event Action<int> TaskUpdated;

        public event Action<int, StatusEnums> StatusUpdated;

        public TaskInfo GetTaskInfo(int accountId)
        {
            var task = _tasks.GetValueOrDefault(accountId);
            if (task is null)
            {
                task = new();
                _tasks.Add(accountId, task);
            }
            return task;
        }

        public List<TaskBase> GetTaskList(int accountId)
        {
            var taskInfo = GetTaskInfo(accountId);
            return taskInfo.TaskList;
        }

        public StatusEnums GetStatus(int accountId)
        {
            var taskInfo = GetTaskInfo(accountId);
            return taskInfo.Status;
        }

        public void SetStatus(int accountId, StatusEnums status)
        {
            var taskInfo = GetTaskInfo(accountId);
            taskInfo.Status = status;
            StatusUpdated?.Invoke(accountId, status);
        }

        public bool IsExecuting(int accountId)
        {
            var taskInfo = GetTaskInfo(accountId);
            return taskInfo.IsExecuting;
        }

        public CancellationTokenSource GetCancellationTokenSource(int accountId)
        {
            var taskInfo = GetTaskInfo(accountId);
            return taskInfo.CancellationTokenSource;
        }

        public TaskBase GetCurrentTask(int accountId)
        {
            var tasks = GetTaskList(accountId);
            return tasks.FirstOrDefault(x => x.Stage == StageEnums.Executing);
        }

        public void Add<T>(int accountId, bool first = false) where T : AccountTask
        {
            var task = Locator.Current.GetService<T>();
            task.Setup(accountId);
            Add(accountId, task, first);
        }

        public void Add<T>(int accountId, int villageId, bool first = false) where T : VillageTask
        {
            var task = Locator.Current.GetService<T>();
            task.Setup(accountId, villageId);
            Add(accountId, task, first);
        }

        public AccountTask Get<T>(int accountId) where T : AccountTask
        {
            var tasks = GetTaskList(accountId);
            var filteredTasks = tasks.OfType<T>();
            var task = filteredTasks.FirstOrDefault(x => x.AccountId == accountId);
            return task;
        }

        public VillageTask Get<T>(int accountId, int villageId) where T : VillageTask
        {
            var tasks = GetTaskList(accountId);
            var filteredTasks = tasks.OfType<T>();
            var task = filteredTasks.FirstOrDefault(x => x.AccountId == accountId && x.VillageId == villageId);
            return task;
        }

        private void Add(int accountId, TaskBase task, bool first = false)
        {
            var tasks = GetTaskList(accountId);

            if (first)
            {
                var firstTask = tasks.FirstOrDefault();
                if (firstTask is not null)
                {
                    task.ExecuteAt = firstTask.ExecuteAt.AddSeconds(-1);
                }
            }

            if (task.ExecuteAt == default) task.ExecuteAt = DateTime.Now;

            tasks.Add(task);
            ReOrder(accountId, tasks);
        }

        public void ReOrder(int accountId)
        {
            var tasks = GetTaskList(accountId);
            ReOrder(accountId, tasks);
        }

        public void Remove(int accountId, TaskBase task)
        {
            var tasks = GetTaskList(accountId);
            tasks.Remove(task);
            ReOrder(accountId, tasks);
        }

        public void Clear(int accountId)
        {
            var tasks = GetTaskList(accountId);
            tasks.Clear();
            TaskUpdated?.Invoke(accountId);
        }

        private void ReOrder(int accountId, List<TaskBase> tasks)
        {
            tasks.Sort((x, y) => DateTime.Compare(x.ExecuteAt, y.ExecuteAt));
            TaskUpdated?.Invoke(accountId);
        }
    }
}