using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs.Villages;

namespace WPFUI.Views.Tabs.Villages
{
    public class BuildTabBase : ReactiveUserControl<BuildViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for BuildTab.xaml
    /// </summary>
    public partial class BuildTab : BuildTabBase
    {
        public BuildTab()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Buildings, v => v.BuildingsGrid.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedBuilding, v => v.BuildingsGrid.SelectedItem).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Jobs, v => v.JobsGrid.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedJob, v => v.JobsGrid.SelectedItem).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedJobIndex, v => v.JobsGrid.SelectedIndex).DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.UpCommand, v => v.UpButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DownCommand, v => v.DownButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.TopCommand, v => v.TopButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.BottomCommand, v => v.BottomButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DeleteCommand, v => v.DeleteButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DeleteAllCommand, v => v.DeleteAllButton).DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.NormalBuildCommand, v => v.NormalBuild).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.NormalBuildInput.Buildings, v => v.NormalBuildings.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.NormalBuildInput.SelectedBuilding, v => v.NormalBuildings.SelectedItem).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.NormalBuildInput.Level, v => v.NormalLevel.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.IsEnableNormalBuild, v => v.NormalBuildings.IsEnabled).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.IsEnableNormalBuild, v => v.NormalLevel.IsEnabled).DisposeWith(d);
            });
        }
    }
}