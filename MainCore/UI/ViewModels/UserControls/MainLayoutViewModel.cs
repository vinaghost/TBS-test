using MainCore.Common.Enums;
using MainCore.Common.Extensions;
using MainCore.Common.Repositories;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.UI.Commands;
using MainCore.UI.Enums;
using MainCore.UI.Models.Output;
using MainCore.UI.Stores;
using MainCore.UI.ViewModels.Abstract;
using MediatR;
using ReactiveUI;
using System.Reactive.Linq;
using Unit = System.Reactive.Unit;

namespace MainCore.UI.ViewModels.UserControls
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class MainLayoutViewModel : ViewModelBase
    {
        private readonly IAccountRepository _accountRepository;

        private readonly ITaskManager _taskManager;
        private readonly IMediator _mediator;

        private readonly AccountTabStore _accountTabStore;
        private readonly SelectedItemStore _selectedItemStore;
        private readonly IDialogService _dialogService;

        public ListBoxItemViewModel Accounts { get; } = new();
        public AccountTabStore AccountTabStore => _accountTabStore;

        public MainLayoutViewModel(IAccountRepository accountRepository, AccountTabStore accountTabStore, SelectedItemStore selectedItemStore, ITaskManager taskManager, IMediator mediator, IDialogService dialogService)
        {
            _accountTabStore = accountTabStore;
            _selectedItemStore = selectedItemStore;

            _accountRepository = accountRepository;

            _taskManager = taskManager;
            _mediator = mediator;

            AddAccountCommand = ReactiveCommand.CreateFromTask(AddAccountTask);
            AddAccountsCommand = ReactiveCommand.CreateFromTask(AddAccountsTask);

            DeleteAccountCommand = ReactiveCommand.CreateFromTask(DeleteAccountTask);
            LoginCommand = ReactiveCommand.CreateFromTask(LoginTask);
            LogoutCommand = ReactiveCommand.CreateFromTask(LogoutTask);
            PauseCommand = ReactiveCommand.CreateFromTask(PauseTask);
            RestartCommand = ReactiveCommand.CreateFromTask(RestartTask);

            var accountObservable = this.WhenAnyValue(x => x.Accounts.SelectedItem);
            accountObservable.BindTo(_selectedItemStore, vm => vm.Account);

            accountObservable.Subscribe(x =>
            {
                var tabType = AccountTabType.Normal;
                if (x is null) tabType = AccountTabType.NoAccount;
                _accountTabStore.SetTabType(tabType);
            });
            _dialogService = dialogService;
        }

        public async Task Load()
        {
            await LoadAccountList();
        }

        private Task AddAccountTask()
        {
            Accounts.SelectedItem = null;
            AccountTabStore.SetTabType(AccountTabType.AddAccount);
            return Task.CompletedTask;
        }

        private Task AddAccountsTask()
        {
            Accounts.SelectedItem = null;
            AccountTabStore.SetTabType(AccountTabType.AddAccounts);
            return Task.CompletedTask;
        }

        private async Task DeleteAccountTask()
        {
            if (!Accounts.IsSelected)
            {
                _dialogService.ShowMessageBox("Warning", "No account selected");
                return;
            }

            var result = _dialogService.ShowConfirmBox("Information", $"Are you sure want to delete \n {Accounts.SelectedItem.Content}");
            if (!result) return;
            await _accountRepository.DeleteById(Accounts.SelectedItemId);
        }

        private async Task LoginTask()
        {
            if (!Accounts.IsSelected)
            {
                _dialogService.ShowMessageBox("Warning", "No account selected");
                return;
            }
            await _mediator.Send(new LoginCommand(Accounts.SelectedItemId));
        }

        private async Task LogoutTask()
        {
            if (!Accounts.IsSelected)
            {
                _dialogService.ShowMessageBox("Warning", "No account selected");
                return;
            }

            await _mediator.Send(new LogoutCommand(Accounts.SelectedItemId));
        }

        private async Task PauseTask()
        {
            if (!Accounts.IsSelected)
            {
                _dialogService.ShowMessageBox("Warning", "No account selected");
                return;
            }

            await _mediator.Send(new PauseCommand(Accounts.SelectedItemId));
        }

        private async Task RestartTask()
        {
            if (!Accounts.IsSelected)
            {
                _dialogService.ShowMessageBox("Warning", "No account selected");
                return;
            }

            await _mediator.Send(new RestartCommand(Accounts.SelectedItemId));
        }

        public async Task LoadStatus(int accountId, StatusEnums status)
        {
            var account = Accounts.Items.FirstOrDefault(x => x.Id == accountId);
            await Observable.Start(() =>
            {
                account.Color = status.GetColor();
            }, RxApp.MainThreadScheduler);
        }

        public async Task LoadAccountList()
        {
            var accounts = await _accountRepository.GetAll();
            var items = accounts.Select(x => new ListBoxItem(x));

            foreach (var item in items)
            {
                var status = _taskManager.GetStatus(item.Id);
                item.Color = status.GetColor();
            }

            await Observable.Start(() =>
            {
                Accounts.Load(items);
            }, RxApp.MainThreadScheduler);
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