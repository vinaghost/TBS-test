using LoginCore.Tasks;
using MainCore.Commands;
using MainCore.Enums;
using MainCore.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
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

        private readonly IOpenBrowserCommand _openBrowserCommand;
        private readonly ICloseBrowserCommand _closeBrowserCommand;

        private readonly ITaskManager _taskManager;
        private readonly ITimerManager _timerManager;

        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        private readonly AccountTabStore _accountTabStore;
        private readonly SelectedItemStore _selectedItemStore;

        public AccountTabStore AccountTabStore => _accountTabStore;

        public MainLayoutViewModel(IMessageService messageService, IAccountRepository accountRepository, WaitingOverlayViewModel waitingOverlayViewModel, AccountTabStore accountTabStore, SelectedItemStore selectedItemStore, IOpenBrowserCommand openBrowserCommand, ICloseBrowserCommand closeBrowserCommand, ITaskManager taskManager, ITimerManager timerManager)
        {
            _messageService = messageService;
            _accountTabStore = accountTabStore;
            _selectedItemStore = selectedItemStore;
            _waitingOverlayViewModel = waitingOverlayViewModel;

            _accountRepository = accountRepository;

            _openBrowserCommand = openBrowserCommand;
            _closeBrowserCommand = closeBrowserCommand;
            _taskManager = taskManager;

            AddAccountCommand = ReactiveCommand.CreateFromTask(AddAccountTask);
            AddAccountsCommand = ReactiveCommand.CreateFromTask(AddAccountsTask);

            DeleteAccountCommand = ReactiveCommand.CreateFromTask(DeleteAccountTask);
            LoginCommand = ReactiveCommand.CreateFromTask(LoginTask);
            LogoutCommand = ReactiveCommand.CreateFromTask(LogoutTask);
            PauseCommand = ReactiveCommand.CreateFromTask(PauseTask);
            RestartCommand = ReactiveCommand.CreateFromTask(RestartTask);

            _accountRepository.AccountTableChanged += LoadAccountList;

            var accountObservable = this.WhenAnyValue(x => x.SelectedAccount);
            accountObservable.BindTo(_selectedItemStore, vm => vm.Account);

            accountObservable.Subscribe(x =>
            {
                var tabType = TabType.Normal;
                if (x is null) tabType = TabType.NoAccount;
                _accountTabStore.SetTabType(tabType);
            });
            _timerManager = timerManager;
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

            var accountId = SelectedAccount.Id;
            var taskInfo = _taskManager.GetTaskInfo(accountId);
            taskInfo.Status = StatusEnums.Starting;
            await _openBrowserCommand.Execute(accountId);

            _taskManager.Add<LoginTask>(accountId);
            _timerManager.Start(accountId);

            taskInfo.Status = StatusEnums.Online;
        }

        private async Task LogoutTask()
        {
            if (SelectedAccount is null)
            {
                _messageService.Show("Warning", "No account selected");
                return;
            }
            await _closeBrowserCommand.Execute(SelectedAccount.Id);
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