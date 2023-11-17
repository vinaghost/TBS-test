using MainCore.Common.Enums;
using MainCore.Repositories;
using MainCore.Services;
using MainCore.Tasks;
using MediatR;

namespace MainCore.Notification.Handler
{
    public class TriggerBuildingUpdate : INotificationHandler<VillageUpdated>
    {
        private readonly ITaskManager _taskManager;
        private readonly IUnitOfRepository _unitOfRepository;

        public TriggerBuildingUpdate(ITaskManager taskManager, IUnitOfRepository unitOfRepository)
        {
            _taskManager = taskManager;
            _unitOfRepository = unitOfRepository;
        }

        public async Task Handle(VillageUpdated notification, CancellationToken cancellationToken)
        {
            var accountId = notification.AccountId;
            var autoLoadVillageBuilding = await Task.Run(() => _unitOfRepository.AccountSettingRepository.GetBooleanByName(accountId, AccountSettingEnums.AutoLoadVillageBuilding));
            if (!autoLoadVillageBuilding) return;

            var villages = await Task.Run(() => _unitOfRepository.VillageRepository.GetMissingBuildingVillages(accountId));

            foreach (var village in villages)
            {
                _taskManager.AddOrUpdate<UpdateVillageTask>(accountId, village);
            }
        }
    }
}