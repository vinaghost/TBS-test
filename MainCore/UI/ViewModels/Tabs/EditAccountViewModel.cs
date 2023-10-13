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
    public class EditAccountViewModel : AccountTabViewModelBase
    {
        public AccessInput AccessInput { get; } = new();
        private readonly IValidator<AccessInput> _accessInputValidator;
        public AccountInput AccountInput { get; } = new();
        private readonly IValidator<AccountInput> _accountInputValidator;

        private AccessDto _selectedAcess;

        public ReactiveCommand<Unit, Unit> AddAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> EditAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> EditAccountCommand { get; }

        private readonly IAccountRepository _accountRepository;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        public EditAccountViewModel(IAccountRepository accountRepository, WaitingOverlayViewModel waitingOverlayViewModel, IValidator<AccessInput> accessInputValidator, IValidator<AccountInput> accountInputValidator)
        {
            _accountRepository = accountRepository;
            _waitingOverlayViewModel = waitingOverlayViewModel;
            _accessInputValidator = accessInputValidator;
            _accountInputValidator = accountInputValidator;

            AddAccessCommand = ReactiveCommand.CreateFromTask(AddAccessTask);
            EditAccessCommand = ReactiveCommand.CreateFromTask(EditAccessTask);
            DeleteAccessCommand = ReactiveCommand.CreateFromTask(DeleteAccessTask);
            EditAccountCommand = ReactiveCommand.CreateFromTask(EditAccountTask);

            this.WhenAnyValue(vm => vm.SelectedAccess)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    var mapper = new AccessInputMapper();
                    mapper.Map(x, AccessInput);
                });
        }

        protected override async Task Load(int accountId)
        {
            var account = await _accountRepository.Get(accountId);
            var mapper = new AccountInputMapper();
            mapper.Map(account, AccountInput);
            AccountInput.SetAccesses(account.Accesses);
        }

        private Task AddAccessTask()
        {
            var results = _accessInputValidator.Validate(AccessInput);

            if (!results.IsValid)
            {
                //_messageService.Show("Error", results.ToString());
            }
            else
            {
                var mapper = new AccessInputMapper();
                var dto = mapper.Map(AccessInput);
                AccountInput.Accesses.Add(dto);
            }
            return Task.CompletedTask;
        }

        private Task EditAccessTask()
        {
            var result = _accessInputValidator.Validate(AccessInput);

            if (!result.IsValid)
            {
                //_messageService.Show("Error", result.ToString());
            }
            else
            {
                var mapper = new AccessInputMapper();
                mapper.Map(SelectedAccess, AccessInput);
            }
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
                //_messageService.Show("Error", results.ToString());
            }
            else
            {
                _waitingOverlayViewModel.Show("editting account ...");
                var mapper = new AccountInputMapper();
                var dto = mapper.Map(AccountInput);
                await _accountRepository.Edit(dto);
                _waitingOverlayViewModel.Close();
            }
        }

        public AccessDto SelectedAccess
        {
            get => _selectedAcess;
            set => this.RaiseAndSetIfChanged(ref _selectedAcess, value);
        }
    }
}