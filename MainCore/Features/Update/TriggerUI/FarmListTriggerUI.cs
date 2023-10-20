using MainCore.UI.ViewModels.Tabs;
using MediatR;

namespace MainCore.Features.Update.TriggerUI
{
    public class FarmListTriggerUI : IRequest
    {
        public int AccountId { get; }

        public FarmListTriggerUI(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class FarmListTriggerUIHandler : IRequestHandler<FarmListTriggerUI>
    {
        private readonly FarmingViewModel _farmingViewModel;

        public FarmListTriggerUIHandler(FarmingViewModel farmingViewModel)
        {
            _farmingViewModel = farmingViewModel;
        }

        public async Task Handle(FarmListTriggerUI request, CancellationToken cancellationToken)
        {
            await _farmingViewModel.FarmListsUpdated(request.AccountId);
        }
    }
}