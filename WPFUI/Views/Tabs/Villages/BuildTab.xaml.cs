using ReactiveUI;
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
        }
    }
}