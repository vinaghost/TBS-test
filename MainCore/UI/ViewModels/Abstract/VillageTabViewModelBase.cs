using MainCore.Entities;
using MainCore.UI.Stores;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Linq;

namespace MainCore.UI.ViewModels.Abstract
{
    public abstract class VillageTabViewModelBase : TabViewModelBase
    {
        protected readonly SelectedItemStore _selectedItemStore;

        private readonly ObservableAsPropertyHelper<AccountId> _accountId;
        public AccountId AccountId => _accountId.Value;

        private readonly ObservableAsPropertyHelper<VillageId> _villageId;
        public VillageId VillageId => _villageId.Value;

        public ReactiveCommand<VillageId, Unit> VillageIdChangeHandleCommand { get; }

        public VillageTabViewModelBase()
        {
            _selectedItemStore = Locator.Current.GetService<SelectedItemStore>();
            VillageIdChangeHandleCommand = ReactiveCommand.CreateFromTask<VillageId, Unit>(VillageIdChangeHandleTask);

            var accountIdObservable = this.WhenAnyValue(vm => vm._selectedItemStore.Account)
                                       .WhereNotNull()
                                       .Select(x => new AccountId(x.Id));

            accountIdObservable.ToProperty(this, vm => vm.AccountId, out _accountId);

            var villageIdObservable = this.WhenAnyValue(vm => vm._selectedItemStore.Village)
                                        .WhereNotNull()
                                        .Select(x => new VillageId(x.Id));

            villageIdObservable.ToProperty(this, vm => vm.VillageId, out _villageId);
            villageIdObservable.InvokeCommand(VillageIdChangeHandleCommand);
        }

        private async Task<Unit> VillageIdChangeHandleTask(VillageId villageId)
        {
            if (!IsActive) return Unit.Default;
            await Load(villageId);
            return Unit.Default;
        }

        protected override async Task OnActive()
        {
            if (_selectedItemStore.IsVillageNotSelected) return;
            await Load(new VillageId(_selectedItemStore.Village.Id));
        }

        protected abstract Task Load(VillageId villageId);
    }
}