using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    public class EditAccountTabBase : ReactiveUserControl<EditAccountViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for EditAccountTab.xaml
    /// </summary>
    public partial class EditAccountTab : EditAccountTabBase
    {
        public EditAccountTab()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                DataContext = ViewModel;
                this.BindCommand(ViewModel, vm => vm.AddAccessCommand, v => v.AddAccessButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.EditAccessCommand, v => v.EditAccessButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DeleteAccessCommand, v => v.DeleteAccessButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.EditAccountCommand, v => v.EditAccountButton).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.AccountInput.Accesses, v => v.ProxiesDataGrid.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedAccess, v => v.ProxiesDataGrid.SelectedItem).DisposeWith(d);
            });
        }
    }
}