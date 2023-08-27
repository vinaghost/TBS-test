using ReactiveUI;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.UserControls
{
    public class WaitingOverlayViewModel : ViewModelBase
    {
        public WaitingOverlayViewModel()
        {
            Show("is initializing");
        }

        public void Show(string message)
        {
            BusyMessage = message;
        }

        public void Close()
        {
            BusyMessage = null;
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