using MainCore.UI.ViewModels.Tabs;
using MediatR;

namespace MainCore.Features.Update.Trigger
{
    public class FarmListTrigger : INotification
    {
        public int AccountId { get; }

        public FarmListTrigger(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class FarmListTriggerHandler : INotificationHandler<FarmListTrigger>
    {
        private readonly FarmingViewModel _farmingViewModel;

        public FarmListTriggerHandler(FarmingViewModel farmingViewModel)
        {
            _farmingViewModel = farmingViewModel;
        }

        public async Task Handle(FarmListTrigger request, CancellationToken cancellationToken)
        {
            await _farmingViewModel.FarmListsUpdated(request.AccountId);
        }
    }
}