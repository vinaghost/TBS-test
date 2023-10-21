using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.UI.Notification;
using MainCore.UI.ViewModels.Abstract;
using MainCore.UI.ViewModels.UserControls;
using MediatR;
using ReactiveUI;
using Splat;

namespace MainCore.UI.ViewModels
{
    [RegisterAsTransient(withoutInterface: true)]
    public class MainViewModel : ViewModelBase
    {
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;
        private readonly MessageBoxViewModel _messageBoxViewModel;
        private MainLayoutViewModel _mainLayoutViewModel;

        private readonly IMediator _mediator;

        public MainViewModel(WaitingOverlayViewModel waitingOverlayViewModel, IMediator mediator, MessageBoxViewModel messageBoxViewModel)
        {
            _waitingOverlayViewModel = waitingOverlayViewModel;
            _mediator = mediator;
            _messageBoxViewModel = messageBoxViewModel;
        }

        public async Task Load()
        {
            await _mediator.Publish(new MainWindowLoaded());
            MainLayoutViewModel = Locator.Current.GetService<MainLayoutViewModel>();
            await MainLayoutViewModel.Load();
        }

        public async Task Unload()
        {
            await _mediator.Publish(new MainWindowUnloaded());
        }

        public WaitingOverlayViewModel WaitingOverlayViewModel => _waitingOverlayViewModel;
        public MessageBoxViewModel MessageBoxViewModel => _messageBoxViewModel;

        public MainLayoutViewModel MainLayoutViewModel
        {
            get => _mainLayoutViewModel;
            set => this.RaiseAndSetIfChanged(ref _mainLayoutViewModel, value);
        }
    }
}