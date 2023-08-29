using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.UserControls;

namespace WPFUI.Views.UserControls
{
    public class MainLayoutUcBase : ReactiveUserControl<MainLayoutViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for MainLayoutUc.xaml
    /// </summary>
    public partial class MainLayoutUc : MainLayoutUcBase
    {
        public MainLayoutUc()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                // commands
                this.BindCommand(ViewModel, vm => vm.AddAccountCommand, v => v.AddAccountButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.AddAccountsCommand, v => v.AddAccountsButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LoginCommand, v => v.LoginButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LogoutCommand, v => v.LogoutButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DeleteAccountCommand, v => v.DeleteButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.PauseCommand, v => v.PauseButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.RestartCommand, v => v.RestartButton).DisposeWith(d);

                // account list
                this.OneWayBind(ViewModel, vm => vm.Accounts, v => v.AccountGrid.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedAccount, v => v.AccountGrid.SelectedItem).DisposeWith(d);

                // tabs
                this.Bind(ViewModel, vm => vm.AddAccountViewModel, v => v.AddAccount.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.AddAccountsViewModel, v => v.AddAccounts.ViewModel).DisposeWith(d);
            });
        }
    }
}