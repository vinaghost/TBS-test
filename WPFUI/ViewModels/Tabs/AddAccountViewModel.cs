using System.Threading.Tasks;

namespace WPFUI.ViewModels.Tabs
{
    public class AddAccountViewModel : TabBaseViewModel
    {
        public AccessInput AccessInput { get; } = new();
        private readonly IValidator<AccessInput> _accessInputValidator;
        public AccountInput AccountInput { get; } = new();
        private readonly IValidator<AccountInput> _accountInputValidator;

        private AccessInput _selectedAcess;

        public ReactiveCommand<Unit, Unit> AddAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> EditAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> AddAccountCommand { get; }

        private readonly IAccountRepository _accountRepository;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;
        private readonly IMessageService _messageService;

        public AddAccountViewModel(IAccountRepository accountRepository, WaitingOverlayViewModel waitingOverlayViewModel, IValidator<AccessInput> accessInputValidator, IValidator<AccountInput> accountInputValidator, IMessageService messageService)
        {
            _accountRepository = accountRepository;
            _waitingOverlayViewModel = waitingOverlayViewModel;

            _accessInputValidator = accessInputValidator;
            _accountInputValidator = accountInputValidator;
            _messageService = messageService;

            AddAccessCommand = ReactiveCommand.CreateFromTask(AddAccessTask);
            EditAccessCommand = ReactiveCommand.CreateFromTask(EditAccessTask);
            DeleteAccessCommand = ReactiveCommand.CreateFromTask(DeleteAccessTask);
            AddAccountCommand = ReactiveCommand.CreateFromTask(AddAccountTask);
            this.WhenAnyValue(vm => vm.SelectedAcess)
                .WhereNotNull()
                .Subscribe(x => x.CopyTo(AccessInput));
        }

        private Task AddAccessTask()
        {
            var results = _accessInputValidator.Validate(AccessInput);

            if (!results.IsValid)
            {
                _messageService.Show("Error", results.ToString());
            }
            else
            {
                AccountInput.Accesses.Add(new(AccessInput));
            }
            return Task.CompletedTask;
        }

        private Task EditAccessTask()
        {
            var results = _accessInputValidator.Validate(AccessInput);

            if (!results.IsValid)
            {
                _messageService.Show("Error", results.ToString());
            }
            else
            {
                AccessInput.CopyTo(SelectedAcess);
            }
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
            var results = _accountInputValidator.Validate(AccountInput);

            if (!results.IsValid)
            {
                _messageService.Show("Error", results.ToString());
            }
            else
            {
                _waitingOverlayViewModel.Show("adding account ...");
                await _accountRepository.Add(AccountInput);
                AccountInput.Clear();
                _waitingOverlayViewModel.Close();
            }
        }

        public AccessInput SelectedAcess
        {
            get => _selectedAcess;
            set => this.RaiseAndSetIfChanged(ref _selectedAcess, value);
        }
    }
}