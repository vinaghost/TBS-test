using MainCore.UI.ViewModels.Tabs;
using ReactiveUI;
using System.Reactive.Disposables;

namespace WPFUI.Views.Tabs
{
    public class AccountSettingTabBase : ReactiveUserControl<AccountSettingViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for AccountSettingTab.xaml
    /// </summary>
    public partial class AccountSettingTab : AccountSettingTabBase
    {
        public AccountSettingTab()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.ExportCommand, v => v.ExportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ImportCommand, v => v.ImportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.AccountSettingInput.ClickDelay, v => v.ClickDelay.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.AccountSettingInput.TaskDelay, v => v.TaskDelay.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.AccountSettingInput.IsAutoLoadVillage, v => v.IsAutoLoadVillage.IsChecked).DisposeWith(d);
            });
        }
    }
}