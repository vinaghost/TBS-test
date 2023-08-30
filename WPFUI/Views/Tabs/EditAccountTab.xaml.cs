using ReactiveUI;
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
        }
    }
}