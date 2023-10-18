using MainCore.UI.ViewModels.Tabs;
using MediatR;

namespace MainCore.Common.Requests
{
    public class VillageChange : IRequest
    {
        public int AccountId { get; }

        public VillageChange(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class VillageChangeHandler : IRequestHandler<VillageChange>
    {
        private readonly VillageViewModel _villageViewModel;

        public VillageChangeHandler(VillageViewModel villageViewModel)
        {
            _villageViewModel = villageViewModel;
        }

        public async Task Handle(VillageChange request, CancellationToken cancellationToken)
        {
            await _villageViewModel.VillageUpdate(request.AccountId);
        }
    }
}