using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;
using WPFUI.Models.Input;
using WPFUI.Repositories;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.UserControls;

namespace WPFUI.ViewModels.Tabs
{
    public class EditAccountViewModel : AccountTabBaseViewModel
    {
        public AccessInput AccessInput { get; } = new();
        public AccountInput AccountInput { get; } = new();

        private AccessInput _selectedAcess;

        public ReactiveCommand<Unit, Unit> AddAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> EditAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> EditAccountCommand { get; }

        private readonly IAccountRepository _accountRepository;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        public EditAccountViewModel(IAccountRepository accountRepository, WaitingOverlayViewModel waitingOverlayViewModel)
        {
            _accountRepository = accountRepository;
            _waitingOverlayViewModel = waitingOverlayViewModel;

            AddAccessCommand = ReactiveCommand.CreateFromTask(AddAccessTask);
            EditAccessCommand = ReactiveCommand.CreateFromTask(EditAccessTask);
            EditAccountCommand = ReactiveCommand.CreateFromTask(EditAccountTask);

            this.WhenAnyValue(vm => vm.SelectedAcess)
                .WhereNotNull()
                .Subscribe(x => x.CopyTo(AccessInput));
        }

        protected override async Task Load(int accountId)
        {
            var account = await _accountRepository.Get(accountId);
            AccountInput.CopyFrom(account);
        }

        private Task AddAccessTask()
        {
            AccountInput.Accesses.Add(new(AccessInput));
            return Task.CompletedTask;
        }

        private Task EditAccessTask()
        {
            AccessInput.CopyTo(SelectedAcess);
            return Task.CompletedTask;
        }

        private async Task EditAccountTask()
        {
            _waitingOverlayViewModel.Show("editting account ...");
            //await Observable.StartAsync(() => _accountRepository.Add(AccountInput), RxApp.TaskpoolScheduler);
            await Task.Delay(200);
            AccountInput.Clear();
            AccessInput.Clear();
            _waitingOverlayViewModel.Close();
        }

        public AccessInput SelectedAcess
        {
            get => _selectedAcess;
            set => this.RaiseAndSetIfChanged(ref _selectedAcess, value);
        }
    }
}