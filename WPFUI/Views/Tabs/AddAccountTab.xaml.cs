using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    public class AddAccountTabBase : ReactiveUserControl<AddAccountViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for AddAccountTab.xaml
    /// </summary>
    public partial class AddAccountTab : AddAccountTabBase
    {
        public AddAccountTab()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.AddAccessCommand, v => v.AddAccessButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.EditAccessCommand, v => v.EditAccessButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DeleteAccessCommand, v => v.DeleteAccessButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.AddAccountCommand, v => v.AddAccountButton).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.AccountInput.Accesses, v => v.ProxiesDataGrid.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedAcess, v => v.ProxiesDataGrid.SelectedItem).DisposeWith(d);
            });
        }
    }
}