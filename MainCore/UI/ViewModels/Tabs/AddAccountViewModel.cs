using FluentValidation;
using MainCore.Common.Repositories;
using MainCore.DTO;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.UI.Models.Input;
using MainCore.UI.ViewModels.Abstract;
using MainCore.UI.ViewModels.UserControls;
using ReactiveUI;
using System.Reactive;

namespace MainCore.UI.ViewModels.Tabs
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class AddAccountViewModel : TabViewModelBase
    {
        public AccessInput AccessInput { get; } = new();
        private readonly IValidator<AccessInput> _accessInputValidator;
        public AccountInput AccountInput { get; } = new();
        private readonly IValidator<AccountInput> _accountInputValidator;

        private AccessDto _selectedAccess;
        private readonly MessageBoxViewModel _messageBoxViewModel;

        public ReactiveCommand<Unit, Unit> AddAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> EditAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> AddAccountCommand { get; }

        private readonly IAccountRepository _accountRepository;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        public AddAccountViewModel(IAccountRepository accountRepository, WaitingOverlayViewModel waitingOverlayViewModel, IValidator<AccessInput> accessInputValidator, IValidator<AccountInput> accountInputValidator, MessageBoxViewModel messageBoxViewModel)
        {
            _accountRepository = accountRepository;
            _waitingOverlayViewModel = waitingOverlayViewModel;

            _accessInputValidator = accessInputValidator;
            _accountInputValidator = accountInputValidator;
            _messageBoxViewModel = messageBoxViewModel;

            AddAccessCommand = ReactiveCommand.CreateFromTask(AddAccessTask);
            EditAccessCommand = ReactiveCommand.CreateFromTask(EditAccessTask);
            DeleteAccessCommand = ReactiveCommand.CreateFromTask(DeleteAccessTask);
            AddAccountCommand = ReactiveCommand.CreateFromTask(AddAccountTask);
            this.WhenAnyValue(vm => vm.SelectedAccess)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    var mapper = new AccessInputMapper();
                    mapper.Map(x, AccessInput);
                });
        }

        private async Task AddAccessTask()
        {
            var result = _accessInputValidator.Validate(AccessInput);

            if (!result.IsValid)
            {
                await _messageBoxViewModel.Show("Error", result.ToString());
            }
            else
            {
                var mapper = new AccessInputMapper();
                var dto = mapper.Map(AccessInput);
                AccountInput.Accesses.Add(dto);
            }
        }

        private async Task EditAccessTask()
        {
            var result = _accessInputValidator.Validate(AccessInput);

            if (!result.IsValid)
            {
                await _messageBoxViewModel.Show("Error", result.ToString());
            }
            else
            {
                var mapper = new AccessInputMapper();
                mapper.Map(SelectedAccess, AccessInput);
            }
        }

        private Task DeleteAccessTask()
        {
            AccountInput.Accesses.Remove(SelectedAccess);
            SelectedAccess = null;
            return Task.CompletedTask;
        }

        private async Task AddAccountTask()
        {
            var results = _accountInputValidator.Validate(AccountInput);

            if (!results.IsValid)
            {
                await _messageBoxViewModel.Show("Error", results.ToString());
            }
            else
            {
                var mapper = new AccountInputMapper();
                var dto = mapper.Map(AccountInput);
                await _waitingOverlayViewModel.Show(
                    "adding account ...",
                    () => _accountRepository.Add(dto)
                );
                await _messageBoxViewModel.Show("Information", "Added account");
            }
        }

        public AccessDto SelectedAccess
        {
            get => _selectedAccess;
            set => this.RaiseAndSetIfChanged(ref _selectedAccess, value);
        }
    }
}