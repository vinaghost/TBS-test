using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.UI.ViewModels.Abstract;
using ReactiveUI;

namespace MainCore.UI.ViewModels.UserControls
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class WaitingOverlayViewModel : ViewModelBase
    {
        private readonly MessageBoxViewModel _messageBoxViewModel;

        public WaitingOverlayViewModel(MessageBoxViewModel messageBoxViewModel)
        {
            _messageBoxViewModel = messageBoxViewModel;
            BusyMessage = "is initializing";
        }

        public async Task<bool> Show(string message, Func<Task> func)
        {
            try
            {
                BusyMessage = message;
                Shown = true;
                await func();
                Shown = false;
                return true;
            }
            catch (Exception ex)
            {
                await _messageBoxViewModel.Show("Error", $"{ex.Message} \n {ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> Show(string message, Action func)
        {
            return await Show(message, () => Task.Run(func));
        }

        private bool _shown;

        public bool Shown
        {
            get => _shown;
            set => this.RaiseAndSetIfChanged(ref _shown, value);
        }

        private string _busyMessage;

        public string BusyMessage
        {
            get => _busyMessage;
            set
            {
                var formattedValue = string.IsNullOrWhiteSpace(value) ? value : $"TBS is {value} ...";
                this.RaiseAndSetIfChanged(ref _busyMessage, formattedValue);
            }
        }
    }
}