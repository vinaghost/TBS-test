using ReactiveUI;
using Splat;
using System.Threading.Tasks;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.UserControls;

namespace WPFUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;
        private MainLayoutViewModel _mainLayoutViewModel;

        public MainViewModel(WaitingOverlayViewModel waitingOverlayViewModel)
        {
            _waitingOverlayViewModel = waitingOverlayViewModel;
        }

        public async Task Load()
        {
            await Task.Delay(2000);
            MainLayoutViewModel = Locator.Current.GetService<MainLayoutViewModel>();
            _waitingOverlayViewModel.Close();
        }

        public async Task Unload()
        {
            _waitingOverlayViewModel.Show("shutting down");
            await Task.Delay(2000);
        }

        public WaitingOverlayViewModel WaitingOverlayViewModel => _waitingOverlayViewModel;

        public MainLayoutViewModel MainLayoutViewModel
        {
            get => _mainLayoutViewModel;
            set => this.RaiseAndSetIfChanged(ref _mainLayoutViewModel, value);
        }
    }
}