using MainCore.UI.ViewModels.Tabs;
using MediatR;

namespace MainCore.Features.Update.TriggerUI
{
    public class VillageTriggerUI : IRequest
    {
        public int AccountId { get; }

        public VillageTriggerUI(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class VillageTriggerUIHandler : IRequestHandler<VillageTriggerUI>
    {
        private readonly VillageViewModel _villageViewModel;

        public VillageTriggerUIHandler(VillageViewModel villageViewModel)
        {
            _villageViewModel = villageViewModel;
        }

        public async Task Handle(VillageTriggerUI request, CancellationToken cancellationToken)
        {
            await _villageViewModel.VillageUpdate(request.AccountId);
        }
    }
}