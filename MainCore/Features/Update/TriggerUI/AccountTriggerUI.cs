using MainCore.UI.ViewModels.UserControls;
using MediatR;

namespace MainCore.Features.Update.TriggerUI
{
    public class AccountTriggerUI : IRequest
    {
    }

    public class AccountTriggerUIHandler : IRequestHandler<AccountTriggerUI>
    {
        private readonly MainLayoutViewModel _mainLayoutViewModel;

        public AccountTriggerUIHandler(MainLayoutViewModel mainLayoutViewModel)
        {
            _mainLayoutViewModel = mainLayoutViewModel;
        }

        public async Task Handle(AccountTriggerUI request, CancellationToken cancellationToken)
        {
            await _mainLayoutViewModel.AccountUpdate();
        }
    }
}