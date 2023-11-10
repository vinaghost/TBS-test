using MainCore.Common;
using MainCore.Common.Enums;
using MainCore.Features.Update.Tasks;
using MainCore.Infrasturecture.Services;
using MediatR;

namespace MainCore.Notification.Handler
{
    public class TriggerBuildingUpdate : INotificationHandler<VillageUpdated>
    {
        private readonly ITaskManager _taskManager;
        private readonly IUnitOfWork _unitOfWork;

        public TriggerBuildingUpdate(ITaskManager taskManager, IUnitOfWork unitOfWork)
        {
            _taskManager = taskManager;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(VillageUpdated notification, CancellationToken cancellationToken)
        {
            var accountId = notification.AccountId;
            var autoLoadVillageBuilding = await Task.Run(() => _unitOfWork.AccountSettingRepository.GetBooleanByName(accountId, AccountSettingEnums.AutoLoadVillageBuilding));
            if (!autoLoadVillageBuilding) return;

            var villages = await Task.Run(() => _unitOfWork.VillageRepository.GetMissingBuildingVillages(accountId));

            foreach (var village in villages)
            {
                _taskManager.AddOrUpdate<UpdateVillageTask>(accountId, village);
            }
        }
    }
}