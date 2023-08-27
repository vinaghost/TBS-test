using ReactiveUI;
using WPFUI.ViewModels.UserControls;

namespace WPFUI.Views.UserControls
{
    public class WaitingOverlayUcBase : ReactiveUserControl<WaitingOverlayViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for WaitingOverlayUc.xaml
    /// </summary>
    public partial class WaitingOverlayUc : WaitingOverlayUcBase
    {
        public WaitingOverlayUc()
        {
            InitializeComponent();
        }
    }
}