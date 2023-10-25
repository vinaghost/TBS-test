using MainCore.Entities;
using MainCore.UI.Stores;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Linq;

namespace MainCore.UI.ViewModels.Abstract
{
    public abstract class AccountTabViewModelBase : TabViewModelBase
    {
        protected readonly SelectedItemStore _selectedItemStore;

        private readonly ObservableAsPropertyHelper<AccountId> _accountId;
        public AccountId AccountId => _accountId.Value;

        public ReactiveCommand<AccountId, Unit> AccountIdChangeHandleCommand { get; }

        public AccountTabViewModelBase()
        {
            _selectedItemStore = Locator.Current.GetService<SelectedItemStore>();
            AccountIdChangeHandleCommand = ReactiveCommand.CreateFromTask<AccountId, Unit>(AccountIdChangeHandleTask);

            var accountIdObservable = this.WhenAnyValue(vm => vm._selectedItemStore.Account)
                                        .WhereNotNull()
                                        .Select(x => new AccountId(x.Id));

            accountIdObservable.ToProperty(this, vm => vm.AccountId, out _accountId);
            accountIdObservable.InvokeCommand(AccountIdChangeHandleCommand);
        }

        private async Task<Unit> AccountIdChangeHandleTask(AccountId accountId)
        {
            if (!IsActive) return Unit.Default;
            await Load(accountId);
            return Unit.Default;
        }

        protected override async Task OnActive()
        {
            if (_selectedItemStore.IsAccountNotSelected) return;
            await Load(new AccountId(_selectedItemStore.Account.Id));
        }

        protected abstract Task Load(AccountId accountId);
    }
}