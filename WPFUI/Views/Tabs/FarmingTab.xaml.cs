using ReactiveUI;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    public class FarmingTabBase : ReactiveUserControl<FarmingViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for FarmingTab.xaml
    /// </summary>
    public partial class FarmingTab : FarmingTabBase
    {
        public FarmingTab()
        {
            InitializeComponent();
        }
    }
}