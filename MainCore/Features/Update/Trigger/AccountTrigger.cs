using MainCore.UI.ViewModels.UserControls;
using MediatR;

namespace MainCore.Features.Update.Trigger
{
    public class AccountTrigger : INotification
    {
    }

    public class AccountTriggerHandler : INotificationHandler<AccountTrigger>
    {
        private readonly MainLayoutViewModel _mainLayoutViewModel;

        public AccountTriggerHandler(MainLayoutViewModel mainLayoutViewModel)
        {
            _mainLayoutViewModel = mainLayoutViewModel;
        }

        public async Task Handle(AccountTrigger request, CancellationToken cancellationToken)
        {
            await _mainLayoutViewModel.AccountUpdate();
        }
    }
}