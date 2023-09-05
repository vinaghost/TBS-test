using System.Threading.Tasks;

namespace WPFUI.ViewModels.Tabs
{
    public class EditAccountViewModel : AccountTabBaseViewModel
    {
        public AccessInput AccessInput { get; } = new();
        private readonly IValidator<AccessInput> _accessInputValidator;
        public AccountInput AccountInput { get; } = new();
        private readonly IValidator<AccountInput> _accountInputValidator;

        private AccessInput _selectedAcess;
        public ReactiveCommand<Unit, Unit> AddAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> EditAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> EditAccountCommand { get; }

        private readonly IAccountRepository _accountRepository;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;
        private readonly IMessageService _messageService;

        public EditAccountViewModel(IAccountRepository accountRepository, WaitingOverlayViewModel waitingOverlayViewModel, IValidator<AccessInput> accessInputValidator, IValidator<AccountInput> accountInputValidator, IMessageService messageService)
        {
            _accountRepository = accountRepository;
            _waitingOverlayViewModel = waitingOverlayViewModel;
            _accessInputValidator = accessInputValidator;
            _accountInputValidator = accountInputValidator;
            _messageService = messageService;

            AddAccessCommand = ReactiveCommand.CreateFromTask(AddAccessTask);
            EditAccessCommand = ReactiveCommand.CreateFromTask(EditAccessTask);
            DeleteAccessCommand = ReactiveCommand.CreateFromTask(DeleteAccessTask);
            EditAccountCommand = ReactiveCommand.CreateFromTask(EditAccountTask);

            this.WhenAnyValue(vm => vm.SelectedAccess)
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
            AccessInput.CopyTo(SelectedAcess);
            return Task.CompletedTask;
        }

        private Task DeleteAccessTask()
        {
            AccountInput.Accesses.Remove(SelectedAccess);
            SelectedAccess = null;
            return Task.CompletedTask;
        }

        private async Task EditAccountTask()
        {
            var results = _accountInputValidator.Validate(AccountInput);

            if (!results.IsValid)
            {
                _messageService.Show("Error", results.ToString());
            }
            else
            {
                _waitingOverlayViewModel.Show("editting account ...");
                await _accountRepository.Edit(AccountInput);
                _waitingOverlayViewModel.Close();
            }
        }

        public AccessInput SelectedAccess
        {
            get => _selectedAcess;
            set => this.RaiseAndSetIfChanged(ref _selectedAcess, value);
        }
    }
}