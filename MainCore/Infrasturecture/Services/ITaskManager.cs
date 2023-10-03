using MainCore.Common.Enums;
using MainCore.Common.Tasks;

namespace MainCore.Infrasturecture.Services
{
    public interface ITaskManager
    {
        event Action<int> TaskUpdated;

        event Action<int, StatusEnums> StatusUpdated;

        void Add<T>(int accountId, bool first = false) where T : AccountTask;

        void Add<T>(int accountId, int villageId, bool first = false) where T : VillageTask;

        void Clear(int accountId);

        AccountTask Get<T>(int accountId) where T : AccountTask;

        VillageTask Get<T>(int accountId, int villageId) where T : VillageTask;

        CancellationTokenSource GetCancellationTokenSource(int accountId);

        TaskBase GetCurrentTask(int accountId);

        StatusEnums GetStatus(int accountId);

        TaskManager.TaskInfo GetTaskInfo(int accountId);

        List<TaskBase> GetTaskList(int accountId);

        void Remove(int accountId, TaskBase task);

        void ReOrder(int accountId);

        void SetStatus(int accountId, StatusEnums status);
    }
}