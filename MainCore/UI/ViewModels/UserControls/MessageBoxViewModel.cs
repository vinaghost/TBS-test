using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.UI.ViewModels.Abstract;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace MainCore.UI.ViewModels.UserControls
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class MessageBoxViewModel : ViewModelBase
    {
        #region Methods

        public async Task Show(string header, string message)
        {
            DialogYesNo = false;
            DialogHeader = header;
            DialogMessage = message;
            DialogShown = true;
            await Task.Run(_signal.Wait);
            _signal.Reset();
        }

        public async Task<bool> ShowConfirm(string header, string message)
        {
            DialogYesNo = true;
            DialogHeader = header;
            DialogMessage = message;
            DialogShown = true;
            await Task.Run(_signal.Wait);
            _signal.Reset();
            return _dialogYes;
        }

        private void Hide()
        {
            DialogShown = false;
        }

        private void OkTask()
        {
            Hide();
            _signal.Set();
        }

        private void YesTask()
        {
            _dialogYes = true;
            Hide();
            _signal.Set();
        }

        private void NoTask()
        {
            _dialogYes = false;
            Hide();
            _signal.Set();
        }

        #endregion Methods

        #region Members

        private readonly ManualResetEventSlim _signal = new();
        public ReactiveCommand<Unit, Unit> OkCommand { get; }
        public ReactiveCommand<Unit, Unit> YesCommand { get; }
        public ReactiveCommand<Unit, Unit> NoCommand { get; }

        private bool _dialogShown;
        private string _dialogMessage;
        private string _dialogHeader;
        private bool _dialogYesNo;
        private readonly ObservableAsPropertyHelper<bool> _dialogOk;
        private bool _dialogYes;

        public MessageBoxViewModel()
        {
            OkCommand = ReactiveCommand.Create(OkTask);
            YesCommand = ReactiveCommand.Create(YesTask);
            NoCommand = ReactiveCommand.Create(NoTask);

            this.WhenAnyValue(vm => vm.DialogYesNo)
                .Select(x => !x)
                .ToProperty(this, vm => vm.DialogOk, out _dialogOk);
        }

        public string DialogMessage
        {
            get => _dialogMessage;
            set => this.RaiseAndSetIfChanged(ref _dialogMessage, value);
        }

        public string DialogHeader
        {
            get => _dialogHeader;
            set => this.RaiseAndSetIfChanged(ref _dialogHeader, value);
        }

        public bool DialogShown
        {
            get => _dialogShown;
            set => this.RaiseAndSetIfChanged(ref _dialogShown, value);
        }

        public bool DialogYesNo
        {
            get => _dialogYesNo;
            set => this.RaiseAndSetIfChanged(ref _dialogYesNo, value);
        }

        public bool DialogOk => _dialogOk.Value;

        #endregion Members
    }
}