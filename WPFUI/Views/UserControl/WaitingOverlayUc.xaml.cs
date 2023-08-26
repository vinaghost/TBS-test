using ReactiveUI;
using WPFUI.ViewModels.UserControl;

namespace WPFUI.Views.UserControl
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