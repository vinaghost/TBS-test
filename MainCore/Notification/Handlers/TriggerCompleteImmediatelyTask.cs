using MainCore.Notification.Message;
using MainCore.Services;
using MainCore.Tasks;
using MediatR;

namespace MainCore.Notification.Handlers
{
    public class TriggerCompleteImmediatelyTask : INotificationHandler<BuildingQueueFull>
    {
        private readonly ITaskManager _taskManager;

        public TriggerCompleteImmediatelyTask(ITaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        public async Task Handle(BuildingQueueFull notification, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var accountId = notification.AccountId;
            var villageId = notification.VillageId;
            _taskManager.AddOrUpdate<CompleteImmediatelyTask>(accountId, villageId);
        }
    }
}