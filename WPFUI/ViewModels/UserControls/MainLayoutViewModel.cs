using MainCore.Commands;
using MainCore.Enums;
using MainCore.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WPFUI.Commands;
using WPFUI.Enums;
using WPFUI.Models.Output;
using WPFUI.Repositories;
using WPFUI.Services;
using WPFUI.Stores;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.UserControls
{
    public class MainLayoutViewModel : ViewModelBase
    {
        private readonly IMessageService _messageService;
        private readonly IAccountRepository _accountRepository;

        private readonly ILoginCommand _loginCommand;
        private readonly ILogoutCommand _logoutCommand;
        private readonly IPauseCommand _pauseCommand;
        private readonly IRestartCommand _restartCommand;

        private readonly ITaskManager _taskManager;

        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        private readonly AccountTabStore _accountTabStore;
        private readonly SelectedItemStore _selectedItemStore;

        public AccountTabStore AccountTabStore => _accountTabStore;

        public MainLayoutViewModel(IMessageService messageService, IAccountRepository accountRepository, WaitingOverlayViewModel waitingOverlayViewModel, AccountTabStore accountTabStore, SelectedItemStore selectedItemStore, ICloseBrowserCommand closeBrowserCommand, ITaskManager taskManager, IPauseCommand pauseCommand, IRestartCommand restartCommand, ILoginCommand loginCommand, ILogoutCommand logoutCommand)
        {
            _messageService = messageService;
            _accountTabStore = accountTabStore;
            _selectedItemStore = selectedItemStore;
            _waitingOverlayViewModel = waitingOverlayViewModel;

            _accountRepository = accountRepository;
            _pauseCommand = pauseCommand;
            _restartCommand = restartCommand;
            _loginCommand = loginCommand;
            _logoutCommand = logoutCommand;

            _taskManager = taskManager;

            AddAccountCommand = ReactiveCommand.CreateFromTask(AddAccountTask);
            AddAccountsCommand = ReactiveCommand.CreateFromTask(AddAccountsTask);

            DeleteAccountCommand = ReactiveCommand.CreateFromTask(DeleteAccountTask);
            LoginCommand = ReactiveCommand.CreateFromTask(LoginTask);
            LoginCommand.ThrownExceptions.Subscribe(x => Debug.WriteLine($"{x.Message} {x.StackTrace}"));
            LogoutCommand = ReactiveCommand.CreateFromTask(LogoutTask);
            PauseCommand = ReactiveCommand.CreateFromTask(PauseTask);
            RestartCommand = ReactiveCommand.CreateFromTask(RestartTask);

            _accountRepository.AccountTableChanged += LoadAccountList;
            _taskManager.StatusUpdated += LoadStatusAccountItem;
            var accountObservable = this.WhenAnyValue(x => x.SelectedAccount);
            accountObservable.BindTo(_selectedItemStore, vm => vm.Account);

            accountObservable.Subscribe(x =>
            {
                var tabType = TabType.Normal;
                if (x is null) tabType = TabType.NoAccount;
                _accountTabStore.SetTabType(tabType);
            });
        }

        public async Task Load()
        {
            await LoadAccountList();
        }

        private Task AddAccountTask()
        {
            SelectedAccount = null;
            AccountTabStore.SetTabType(TabType.AddAccount);
            return Task.CompletedTask;
        }

        private Task AddAccountsTask()
        {
            SelectedAccount = null;
            AccountTabStore.SetTabType(TabType.AddAccounts);
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
            await Observable.StartAsync(() => _accountRepository.Delete(SelectedAccount.Id), RxApp.TaskpoolScheduler);

            _waitingOverlayViewModel.Close();
        }

        private async Task LoginTask()
        {
            if (SelectedAccount is null)
            {
                _messageService.Show("Warning", "No account selected");
                return;
            }
            await _loginCommand.Execute(SelectedAccount.Id);
        }

        private async Task LogoutTask()
        {
            if (SelectedAccount is null)
            {
                _messageService.Show("Warning", "No account selected");
                return;
            }
            await _logoutCommand.Execute(SelectedAccount.Id);
        }

        private async Task PauseTask()
        {
            if (SelectedAccount is null)
            {
                _messageService.Show("Warning", "No account selected");
                return;
            }

            await _pauseCommand.Execute(SelectedAccount.Id);
        }

        private async Task RestartTask()
        {
            if (SelectedAccount is null)
            {
                _messageService.Show("Warning", "No account selected");
                return;
            }

            await _restartCommand.Execute(SelectedAccount.Id);
        }

        private void LoadStatusAccountItem(int accountId, StatusEnums status)
        {
            Observable.Start(() =>
            {
                var account = Accounts.FirstOrDefault(x => x.Id == accountId);
                switch (status)
                {
                    case StatusEnums.Online:
                        account.Color = Color.Green.ToMediaColor();
                        break;

                    case StatusEnums.Starting:
                    case StatusEnums.Pausing:
                    case StatusEnums.Stopping:
                        account.Color = Color.Orange.ToMediaColor();
                        break;

                    case StatusEnums.Offline:
                        account.Color = Color.Black.ToMediaColor();
                        break;

                    case StatusEnums.Paused:
                        account.Color = Color.Red.ToMediaColor();
                        break;
                }
            }, RxApp.MainThreadScheduler);
        }

        private async Task LoadAccountList()
        {
            var accounts = await Observable.StartAsync(_accountRepository.Get, RxApp.TaskpoolScheduler);
            Accounts.Clear();
            foreach (var account in accounts)
            {
                Accounts.Add(new(account));
            }

            if (accounts.Count > 0)
            {
                SelectedAccount = Accounts[0];
            }
            else
            {
                SelectedAccount = null;
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