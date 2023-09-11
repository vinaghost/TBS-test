using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WPFUI.Stores;

namespace WPFUI.ViewModels.Abstract
{
    public abstract class VillageTabViewModelBase : TabViewModelBase
    {
        protected readonly SelectedItemStore _selectedItemStore;

        private readonly ObservableAsPropertyHelper<int> _accountId;
        public int AccountId => _accountId.Value;

        private readonly ObservableAsPropertyHelper<int> _villageId;
        public int VillageId => _villageId.Value;

        public ReactiveCommand<int, Unit> VillageIdChangeHandleCommand { get; }

        public VillageTabViewModelBase()
        {
            _selectedItemStore = Locator.Current.GetService<SelectedItemStore>();
            VillageIdChangeHandleCommand = ReactiveCommand.CreateFromTask<int, Unit>(VillageIdChangeHandleTask);

            var accountIdObservable = this.WhenAnyValue(vm => vm._selectedItemStore.Account)
                                       .WhereNotNull()
                                       .Select(x => x.Id);

            accountIdObservable.ToProperty(this, vm => vm.AccountId, out _accountId);

            var villageIdObservable = this.WhenAnyValue(vm => vm._selectedItemStore.Village)
                                        .WhereNotNull()
                                        .Select(x => x.Id);

            villageIdObservable.ToProperty(this, vm => vm.VillageId, out _villageId);
            villageIdObservable.InvokeCommand(VillageIdChangeHandleCommand);
        }

        private async Task<Unit> VillageIdChangeHandleTask(int villageId)
        {
            if (!IsActive) return Unit.Default;
            await Load(villageId);
            return Unit.Default;
        }

        protected override async Task OnActive()
        {
            if (_selectedItemStore.IsVillageNotSelected) return;
            await Load(_selectedItemStore.Village.Id);
        }

        protected abstract Task Load(int villageId);
    }
}