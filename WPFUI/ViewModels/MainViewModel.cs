using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.UserControl;

namespace WPFUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        public MainViewModel(WaitingOverlayViewModel waitingOverlayViewModel)
        {
            _waitingOverlayViewModel = waitingOverlayViewModel;
        }

        public WaitingOverlayViewModel WaitingOverlayViewModel => _waitingOverlayViewModel;
    }
}