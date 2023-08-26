using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.UserControl;

namespace WPFUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;
        private readonly MainLayoutViewModel _mainLayoutViewModel;

        public MainViewModel(WaitingOverlayViewModel waitingOverlayViewModel, MainLayoutViewModel mainLayoutViewModel)
        {
            _waitingOverlayViewModel = waitingOverlayViewModel;
            _mainLayoutViewModel = mainLayoutViewModel;
        }

        public WaitingOverlayViewModel WaitingOverlayViewModel => _waitingOverlayViewModel;
        public MainLayoutViewModel MainLayoutViewModel => _mainLayoutViewModel;
    }
}