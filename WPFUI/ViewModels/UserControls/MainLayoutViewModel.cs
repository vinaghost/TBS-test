using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;
using WPFUI.Services;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.UserControls
{
    public class MainLayoutViewModel : ViewModelBase
    {
        private readonly IMessageService _messageService;

        public MainLayoutViewModel(IMessageService messageService)
        {
            _messageService = messageService;

            AddAccountCommand = ReactiveCommand.CreateFromTask(AddAccountTask);
            AddAccountsCommand = ReactiveCommand.CreateFromTask(AddAccountsTask);

            DeleteAccountCommand = ReactiveCommand.CreateFromTask(DeleteAccountTask);
            LoginCommand = ReactiveCommand.CreateFromTask(LoginTask);
            LogoutCommand = ReactiveCommand.CreateFromTask(LogoutTask);
            PauseCommand = ReactiveCommand.CreateFromTask(PauseTask);
            RestartCommand = ReactiveCommand.CreateFromTask(RestartTask);
        }

        private Task AddAccountTask()
        {
            _messageService.Show("Info", "AddAccountTask");
            return Task.CompletedTask;
        }

        private Task AddAccountsTask()
        {
            _messageService.Show("Info", "AddAccountsTask");
            return Task.CompletedTask;
        }

        private Task DeleteAccountTask()
        {
            _messageService.Show("Info", "DeleteAccountTask");
            return Task.CompletedTask;
        }

        private Task LoginTask()
        {
            _messageService.Show("Info", "LoginTask");
            return Task.CompletedTask;
        }

        private Task LogoutTask()
        {
            _messageService.Show("Info", "LogoutTask");
            return Task.CompletedTask;
        }

        private Task PauseTask()
        {
            _messageService.Show("Info", "PauseTask");
            return Task.CompletedTask;
        }

        private Task RestartTask()
        {
            _messageService.Show("Info", "RestartTask");
            return Task.CompletedTask;
        }

        public ReactiveCommand<Unit, Unit> AddAccountCommand { get; }

        public ReactiveCommand<Unit, Unit> AddAccountsCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
        public ReactiveCommand<Unit, Unit> PauseCommand { get; }
        public ReactiveCommand<Unit, Unit> RestartCommand { get; }
    }
}