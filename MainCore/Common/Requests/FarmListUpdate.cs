using MainCore.UI.ViewModels.Tabs;
using MediatR;

namespace MainCore.Common.Requests
{
    public class FarmListUpdate : IRequest
    {
        public int AccountId { get; }

        public FarmListUpdate(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class FarmListUpdateHandler : IRequestHandler<FarmListUpdate>
    {
        private readonly FarmingViewModel _farmingViewModel;

        public FarmListUpdateHandler(FarmingViewModel farmingViewModel)
        {
            _farmingViewModel = farmingViewModel;
        }

        public async Task Handle(FarmListUpdate request, CancellationToken cancellationToken)
        {
            await _farmingViewModel.FarmListsUpdated(request.AccountId);
        }
    }
}