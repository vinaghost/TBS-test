using MainCore.UI.ViewModels.Tabs.Villages;
using ReactiveUI;
using System.Reactive.Disposables;

namespace WPFUI.Views.Tabs.Villages
{
    public class VillageSettingTabBase : ReactiveUserControl<VillageSettingViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for VillageSettingTab.xaml
    /// </summary>
    public partial class VillageSettingTab : VillageSettingTabBase
    {
        public VillageSettingTab()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.ExportCommand, v => v.ExportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ImportCommand, v => v.ImportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.VillageSettingInput.UseHeroResourceForBuilding, v => v.UseHeroResForBuilding.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.VillageSettingInput.ApplyRomanQueueLogicWhenBuilding, v => v.UseRomanQueueLogic.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.VillageSettingInput.UseSpecialUpgrade, v => v.UseSpecialUpgrade.IsChecked).DisposeWith(d);
            });
        }
    }
}