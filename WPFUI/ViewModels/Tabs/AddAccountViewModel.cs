using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WPFUI.Models.Input;
using WPFUI.Repositories;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.UserControls;

namespace WPFUI.ViewModels.Tabs
{
    public class AddAccountViewModel : TabBaseViewModel
    {
        public AccessInput AccessInput { get; } = new();
        public AccountInput AccountInput { get; } = new();

        private AccessInput _selectedAcess;

        public ReactiveCommand<Unit, Unit> AddAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> EditAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> AddAccountCommand { get; }

        private readonly IAccountRepository _accountRepository;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        public AddAccountViewModel(IAccountRepository accountRepository, WaitingOverlayViewModel waitingOverlayViewModel)
        {
            _accountRepository = accountRepository;
            _waitingOverlayViewModel = waitingOverlayViewModel;

            AddAccessCommand = ReactiveCommand.CreateFromTask(AddAccessTask);
            EditAccessCommand = ReactiveCommand.CreateFromTask(EditAccessTask);
            DeleteAccessCommand = ReactiveCommand.CreateFromTask(DeleteAccessTask);
            AddAccountCommand = ReactiveCommand.CreateFromTask(AddAccountTask);
            AddAccountCommand.ThrownExceptions.Subscribe(x => Debug.WriteLine("{0} {1}", x.Message, x.StackTrace));
            this.WhenAnyValue(vm => vm.SelectedAcess)
                .WhereNotNull()
                .Subscribe(x => x.CopyTo(AccessInput));
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

        private Task DeleteAccessTask()
        {
            AccountInput.Accesses.Remove(SelectedAcess);
            SelectedAcess = null;
            return Task.CompletedTask;
        }

        private async Task AddAccountTask()
        {
            _waitingOverlayViewModel.Show("adding account ...");
            await _accountRepository.Add(AccountInput);
            AccountInput.Clear();
            _waitingOverlayViewModel.Close();
        }

        public AccessInput SelectedAcess
        {
            get => _selectedAcess;
            set => this.RaiseAndSetIfChanged(ref _selectedAcess, value);
        }
    }
}