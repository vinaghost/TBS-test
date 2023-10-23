using MainCore.Common.Enums;
using MainCore.Common.Tasks;
using static MainCore.Infrasturecture.Services.TaskManager;

namespace MainCore.Infrasturecture.Services
{
    public interface ITaskManager
    {
        void Add<T>(int accountId, bool first = false) where T : AccountTask;

        void Add<T>(int accountId, int villageId, bool first = false) where T : VillageTask;

        void AddOrUpdate<T>(int accountId, int villageId, bool first = false) where T : VillageTask;

        void AddOrUpdate<T>(int accountId, bool first = false) where T : AccountTask;

        void Clear(int accountId);

        AccountTask Get<T>(int accountId) where T : AccountTask;

        VillageTask Get<T>(int accountId, int villageId) where T : VillageTask;

        CancellationTokenSource GetCancellationTokenSource(int accountId);

        TaskBase GetCurrentTask(int accountId);

        StatusEnums GetStatus(int accountId);

        TaskInfo GetTaskInfo(int accountId);

        List<TaskBase> GetTaskList(int accountId);

        void Remove(int accountId, TaskBase task);

        void ReOrder(int accountId);

        void SetStatus(int accountId, StatusEnums status);
    }
}