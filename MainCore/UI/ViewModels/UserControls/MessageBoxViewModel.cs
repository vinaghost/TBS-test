using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.UI.ViewModels.Abstract;
using ReactiveUI;

namespace MainCore.UI.ViewModels.UserControls
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class MessageBoxViewModel : ViewModelBase
    {
        private string _title;

        private string _message;
        private bool _isEnable;

        public bool IsEnable
        {
            get => _isEnable;
            set => this.RaiseAndSetIfChanged(ref _isEnable, value);
        }

        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }
    }
}