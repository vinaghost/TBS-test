using MainCore.Notification.Message;
using MainCore.Services;
using MainCore.Tasks;
using MediatR;

namespace MainCore.Notification.Handlers
{
    public class TriggerNPCTask : INotificationHandler<GanaryFull>
    {
        private readonly ITaskManager _taskManager;

        public TriggerNPCTask(ITaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        public async Task Handle(GanaryFull notification, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var accountId = notification.AccountId;
            var villageId = notification.VillageId;
            _taskManager.AddOrUpdate<NPCTask>(accountId, villageId);
        }
    }
}