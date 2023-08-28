using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;
using WPFUI.Models.Input;
using WPFUI.Repositories;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.UserControls;

namespace WPFUI.ViewModels.Tabs
{
    public class AddAccountViewModel : ViewModelBase
    {
        public AccessInput AccessInput { get; } = new();
        public AccountInput AccountInput { get; } = new();
        public ReactiveCommand<Unit, Unit> AddAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> AddAccountCommand { get; }

        private readonly IAccountRepository _accountRepository;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        public AddAccountViewModel(IAccountRepository accountRepository, WaitingOverlayViewModel waitingOverlayViewModel)
        {
            _accountRepository = accountRepository;
            _waitingOverlayViewModel = waitingOverlayViewModel;

            AddAccessCommand = ReactiveCommand.CreateFromTask(AddAccessTask);
            AddAccountCommand = ReactiveCommand.CreateFromTask(AddAccountTask);
        }

        private Task AddAccessTask()
        {
            AccountInput.Accesses.Add(new(AccessInput));
            return Task.CompletedTask;
        }

        private async Task AddAccountTask()
        {
            _waitingOverlayViewModel.Show("adding account ...");
            await _accountRepository.Add(AccountInput);
            AccountInput.Clear();
            AccessInput.Clear();
            _waitingOverlayViewModel.Close();
        }
    }
}