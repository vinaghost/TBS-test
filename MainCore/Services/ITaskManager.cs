using MainCore.Enums;
using MainCore.Tasks;

namespace MainCore.Services
{
    public interface ITaskManager
    {
        void Add<T>(int accountId, bool first = false) where T : AccountTask;

        void Add<T>(int accountId, int villageId, bool first = false) where T : VillageTask;

        CancellationTokenSource GetCancellationTokenSource(int accountId);

        StatusEnums GetStatus(int accountId);

        TaskManager.TaskInfo GetTaskInfo(int accountId);

        List<TaskBase> GetTaskList(int accountId);

        void Remove(int accountId, TaskBase task);

        void ReOrder(int accountId);
    }
}