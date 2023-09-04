using MainCore.Enums;
using MainCore.Tasks;
using Splat;

namespace MainCore.Services
{
    public class TaskManager : ITaskManager
    {
        public class TaskInfo
        {
            public bool IsExecuting { get; set; } = false;
            public StatusEnums Status { get; set; } = StatusEnums.Offline;
            public CancellationTokenSource CancellationTokenSource { get; set; } = null;

            public List<TaskBase> TaskList { get; set; } = new();
        }

        private readonly Dictionary<int, TaskInfo> _tasks = new();

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

        private void ReOrder(int accountId, List<TaskBase> tasks)
        {
            tasks.Sort((x, y) => DateTime.Compare(x.ExecuteAt, y.ExecuteAt));
        }
    }
}