using ReactiveUI;
using System.Linq;
using System.Reactive.Linq;
using WPFUI.Models.Output;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.Stores
{
    public class SelectedItemStore : ViewModelBase
    {
        public SelectedItemStore()
        {
            var accountObservable = this.WhenAnyValue(vm => vm.Account);
            accountObservable
                .Select(x => x is not null)
                .ToProperty(this, vm => vm.IsAccountSelected, out _isAccountSelected);
            accountObservable
                .Select(x => x is null)
                .ToProperty(this, vm => vm.IsAccountNotSelected, out _isAccountNotSelected);
        }

        private ListBoxItem _account;

        public ListBoxItem Account
        {
            get => _account;
            set => this.RaiseAndSetIfChanged(ref _account, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _isAccountSelected;
        private readonly ObservableAsPropertyHelper<bool> _isAccountNotSelected;

        public bool IsAccountSelected => _isAccountSelected.Value;
        public bool IsAccountNotSelected => _isAccountNotSelected.Value;
    }
}