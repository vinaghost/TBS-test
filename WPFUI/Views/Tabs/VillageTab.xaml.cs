using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    public class VillageTabBase : ReactiveUserControl<VillageViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for VillageTab.xaml
    /// </summary>
    public partial class VillageTab : VillageTabBase
    {
        public VillageTab()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Villages, v => v.VillagesGrid.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedVillage, v => v.VillagesGrid.SelectedItem).DisposeWith(d);

                // tabs
                this.OneWayBind(ViewModel, vm => vm.VillageTabStore.NoVillageViewModel, v => v.NoVillage.ViewModel).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.VillageTabStore.InfoViewModel, v => v.Info.ViewModel).DisposeWith(d);

                // visible
                this.OneWayBind(ViewModel, vm => vm.VillageTabStore.IsNoVillageTabVisible, v => v.NoVillageTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.VillageTabStore.IsNormalTabVisible, v => v.InfoTab.Visibility).DisposeWith(d);

                // selected
                this.Bind(ViewModel, vm => vm.VillageTabStore.NoVillageViewModel.IsActive, v => v.NoVillageTab.IsSelected).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.VillageTabStore.InfoViewModel.IsActive, v => v.InfoTab.IsSelected).DisposeWith(d);
            });
        }
    }
}