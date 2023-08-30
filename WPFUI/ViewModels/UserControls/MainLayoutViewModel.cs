using MainCore.Commands;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WPFUI.Models.Output;
using WPFUI.Repositories;
using WPFUI.Services;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.ViewModels.UserControls
{
    public class MainLayoutViewModel : ViewModelBase
    {
        private readonly IMessageService _messageService;
        private readonly IAccountRepository _accountRepository;

        private readonly ILoginCommand _loginCommand;
        private readonly ILogoutCommand _logoutCommand;

        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        private readonly AddAccountViewModel _addAccountViewModel;
        private readonly AddAccountsViewModel _addAccountsViewModel;
        public AddAccountViewModel AddAccountViewModel => _addAccountViewModel;
        public AddAccountsViewModel AddAccountsViewModel => _addAccountsViewModel;

        public MainLayoutViewModel(IMessageService messageService, AddAccountViewModel addAccountViewModel, IAccountRepository accountRepository, WaitingOverlayViewModel waitingOverlayViewModel, AddAccountsViewModel addAccountsViewModel, ILoginCommand loginCommand, ILogoutCommand logoutCommand)
        {
            _messageService = messageService;
            _waitingOverlayViewModel = waitingOverlayViewModel;
            _addAccountViewModel = addAccountViewModel;
            _addAccountsViewModel = addAccountsViewModel;

            _accountRepository = accountRepository;

            _logoutCommand = logoutCommand;
            _loginCommand = loginCommand;

            AddAccountCommand = ReactiveCommand.CreateFromTask(AddAccountTask);
            AddAccountsCommand = ReactiveCommand.CreateFromTask(AddAccountsTask);

            DeleteAccountCommand = ReactiveCommand.CreateFromTask(DeleteAccountTask);
            LoginCommand = ReactiveCommand.CreateFromTask(LoginTask);
            LogoutCommand = ReactiveCommand.CreateFromTask(LogoutTask);
            PauseCommand = ReactiveCommand.CreateFromTask(PauseTask);
            RestartCommand = ReactiveCommand.CreateFromTask(RestartTask);

            _accountRepository.AccountTableChanged += LoadAccountList;
        }

        public async Task Load()
        {
            await LoadAccountList();
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

        private async Task DeleteAccountTask()
        {
            if (SelectedAccount is null)
            {
                _messageService.Show("Warning", "No account selected");
                return;
            }

            var result = _messageService.ShowYesNo("Info", $"Are you sure want to delete \n {SelectedAccount.Content}");
            if (!result) return;

            _waitingOverlayViewModel.Show("deleting account ...");
            await _accountRepository.Delete(SelectedAccount.Id);
            _waitingOverlayViewModel.Close();
        }

        private async Task LoginTask()
        {
            if (SelectedAccount is null)
            {
                _messageService.Show("Warning", "No account selected");
                return;
            }

            await Observable.Start(async () => await _loginCommand.Execute(SelectedAccount.Id), RxApp.TaskpoolScheduler);
        }

        private async Task LogoutTask()
        {
            if (SelectedAccount is null)
            {
                _messageService.Show("Warning", "No account selected");
                return;
            }
            await Observable.Start(async () => await _logoutCommand.Execute(SelectedAccount.Id), RxApp.TaskpoolScheduler);
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

        private async Task LoadAccountList()
        {
            Accounts.Clear();
            var accounts = await _accountRepository.Get();
            foreach (var account in accounts)
            {
                Accounts.Add(new(account));
            }
        }

        public ReactiveCommand<Unit, Unit> AddAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> AddAccountsCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
        public ReactiveCommand<Unit, Unit> PauseCommand { get; }
        public ReactiveCommand<Unit, Unit> RestartCommand { get; }

        public ObservableCollection<ListBoxItem> Accounts { get; } = new();
        private ListBoxItem _selectedAccount;

        public ListBoxItem SelectedAccount
        {
            get => _selectedAccount;
            set => this.RaiseAndSetIfChanged(ref _selectedAccount, value);
        }
    }
}