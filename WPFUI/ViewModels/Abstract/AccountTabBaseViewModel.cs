using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WPFUI.Stores;

namespace WPFUI.ViewModels.Abstract
{
    public abstract class AccountTabBaseViewModel : TabBaseViewModel
    {
        protected readonly SelectedItemStore _selectedItemStore;

        private readonly ObservableAsPropertyHelper<int> _accountId;
        public int AccountId => _accountId.Value;

        public ReactiveCommand<int, Unit> AccountIdChangeHandleCommand { get; }

        public AccountTabBaseViewModel()
        {
            _selectedItemStore = Locator.Current.GetService<SelectedItemStore>();
            AccountIdChangeHandleCommand = ReactiveCommand.CreateFromTask<int, Unit>(AccountIdChangeHandleTask);

            var accountIdObservable = this.WhenAnyValue(vm => vm._selectedItemStore.Account)
                                        .WhereNotNull()
                                        .Select(x => x.Id);

            accountIdObservable.ToProperty(this, vm => vm.AccountId, out _accountId);
            accountIdObservable.InvokeCommand(AccountIdChangeHandleCommand);
        }

        private async Task<Unit> AccountIdChangeHandleTask(int accountId)
        {
            if (!IsActive) return Unit.Default;
            await Load(accountId);
            return Unit.Default;
        }

        protected override async Task OnActive()
        {
            if (_selectedItemStore.IsAccountNotSelected) return;
            await Load(_selectedItemStore.Account.Id);
        }

        protected abstract Task Load(int accountId);
    }
}