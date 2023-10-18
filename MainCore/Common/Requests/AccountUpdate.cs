using MainCore.UI.ViewModels.UserControls;
using MediatR;

namespace MainCore.Common.Requests
{
    public class AccountUpdate : IRequest
    {
    }

    public class AccountChangeHandler : IRequestHandler<AccountUpdate>
    {
        private readonly MainLayoutViewModel _mainLayoutViewModel;

        public AccountChangeHandler(MainLayoutViewModel mainLayoutViewModel)
        {
            _mainLayoutViewModel = mainLayoutViewModel;
        }

        public async Task Handle(AccountUpdate request, CancellationToken cancellationToken)
        {
            await _mainLayoutViewModel.AccountUpdate();
        }
    }
}