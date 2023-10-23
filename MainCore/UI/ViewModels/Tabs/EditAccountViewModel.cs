using FluentValidation;
using MainCore.Common.Repositories;
using MainCore.DTO;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.UI.Models.Input;
using MainCore.UI.ViewModels.Abstract;
using MainCore.UI.ViewModels.UserControls;
using MediatR;
using ReactiveUI;
using System.Reactive.Linq;
using Unit = System.Reactive.Unit;

namespace MainCore.UI.ViewModels.Tabs
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class EditAccountViewModel : AccountTabViewModelBase
    {
        public AccessInput AccessInput { get; } = new();
        private readonly IValidator<AccessInput> _accessInputValidator;
        public AccountInput AccountInput { get; } = new();
        private readonly IValidator<AccountInput> _accountInputValidator;
        private readonly IMediator _mediator;

        private AccessDto _selectedAcess;

        public ReactiveCommand<Unit, Unit> AddAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> EditAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> EditAccountCommand { get; }

        private readonly IAccountRepository _accountRepository;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;
        private readonly MessageBoxViewModel _messageBoxViewModel;

        public EditAccountViewModel(IAccountRepository accountRepository, WaitingOverlayViewModel waitingOverlayViewModel, IValidator<AccessInput> accessInputValidator, IValidator<AccountInput> accountInputValidator, MessageBoxViewModel messageBoxViewModel, IMediator mediator)
        {
            _accountRepository = accountRepository;
            _waitingOverlayViewModel = waitingOverlayViewModel;
            _accessInputValidator = accessInputValidator;
            _accountInputValidator = accountInputValidator;
            _messageBoxViewModel = messageBoxViewModel;

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
            _mediator = mediator;
        }

        protected override async Task Load(int accountId)
        {
            var account = await _accountRepository.GetById(accountId);

            var mapper = new AccountInputMapper();

            await Observable.Start(() =>
            {
                mapper.Map(account, AccountInput);
                AccountInput.SetAccesses(account.Accesses);
            }, RxApp.MainThreadScheduler);
        }

        private async Task AddAccessTask()
        {
            var results = _accessInputValidator.Validate(AccessInput);

            if (!results.IsValid)
            {
                await _messageBoxViewModel.Show("Error", results.ToString());
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

        private async Task EditAccountTask()
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
                    "editting account ...",
                    async () =>
                    {
                        await Task.Run(() => _accountRepository.Edit(dto));
                    });
                await _messageBoxViewModel.Show("Information", "Edited accounts");
            }
        }

        public AccessDto SelectedAccess
        {
            get => _selectedAcess;
            set => this.RaiseAndSetIfChanged(ref _selectedAcess, value);
        }
    }
}