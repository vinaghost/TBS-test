using MainCore.Common.Enums;
using MainCore.Notification.Message;
using MainCore.Repositories;
using MainCore.Services;
using MainCore.Tasks;
using MediatR;

namespace MainCore.Notification.Handlers
{
    public class TriggerRefreshVillageTask : INotificationHandler<VillageSettingUpdated>
    {
        private readonly ITaskManager _taskManager;
        private readonly IUnitOfRepository _unitOfRepository;

        public TriggerRefreshVillageTask(ITaskManager taskManager, IUnitOfRepository unitOfRepository)
        {
            _taskManager = taskManager;
            _unitOfRepository = unitOfRepository;
        }

        public async Task Handle(VillageSettingUpdated notification, CancellationToken cancellationToken)
        {
            var accountId = notification.AccountId;
            var villageId = notification.VillageId;
            var trainTroopEnable = await Task.Run(() => _unitOfRepository.VillageSettingRepository.GetBooleanByName(villageId, VillageSettingEnums.AutoRefreshEnable));
            if (!trainTroopEnable) return;
            _taskManager.AddOrUpdate<UpdateVillageTask>(accountId, villageId);
        }
    }
}