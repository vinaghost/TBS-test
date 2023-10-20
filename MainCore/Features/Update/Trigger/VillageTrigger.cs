using MainCore.UI.ViewModels.Tabs;
using MediatR;

namespace MainCore.Features.Update.Trigger
{
    public class VillageTrigger : INotification
    {
        public int AccountId { get; }

        public VillageTrigger(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class VillageTriggerHandler : INotificationHandler<VillageTrigger>
    {
        private readonly VillageViewModel _villageViewModel;

        public VillageTriggerHandler(VillageViewModel villageViewModel)
        {
            _villageViewModel = villageViewModel;
        }

        public async Task Handle(VillageTrigger request, CancellationToken cancellationToken)
        {
            await _villageViewModel.VillageUpdate(request.AccountId);
        }
    }
}