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
            });
        }
    }
}