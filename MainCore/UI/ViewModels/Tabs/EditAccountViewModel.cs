using FluentValidation;
using MainCore.CQRS.Commands;
using MainCore.CQRS.Queries;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
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

        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;
        private readonly IDialogService _dialogService;

        public EditAccountViewModel(WaitingOverlayViewModel waitingOverlayViewModel, IValidator<AccessInput> accessInputValidator, IValidator<AccountInput> accountInputValidator, IMediator mediator, IDialogService dialogService)
        {
            _waitingOverlayViewModel = waitingOverlayViewModel;
            _accessInputValidator = accessInputValidator;
            _accountInputValidator = accountInputValidator;

            _mediator = mediator;
            _dialogService = dialogService;

            AddAccessCommand = ReactiveCommand.Create(AddAccessCommandHandler);
            EditAccessCommand = ReactiveCommand.Create(EditAccessCommandHandler);
            DeleteAccessCommand = ReactiveCommand.Create(DeleteAccessCommandHandler);
            EditAccountCommand = ReactiveCommand.CreateFromTask(EditAccountCommandHandler);

            this.WhenAnyValue(vm => vm.SelectedAccess)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    var mapper = new AccessInputMapper();
                    mapper.Map(x, AccessInput);
                });
        }

        protected override async Task Load(AccountId accountId)
        {
            await LoadAccount();
        }

        private void AddAccessCommandHandler()
        {
            var result = _accessInputValidator.Validate(AccessInput);

            if (!result.IsValid)
            {
                _dialogService.ShowMessageBox("Error", result.ToString());
                return;
            }

            var mapper = new AccessInputMapper();
            var dto = mapper.Map(AccessInput);
            AccountInput.Accesses.Add(dto);
        }

        private void EditAccessCommandHandler()
        {
            var result = _accessInputValidator.Validate(AccessInput);

            if (!result.IsValid)
            {
                _dialogService.ShowMessageBox("Error", result.ToString());
                return;
            }

            var mapper = new AccessInputMapper();
            mapper.Map(SelectedAccess, AccessInput);
        }

        private void DeleteAccessCommandHandler()
        {
            AccountInput.Accesses.Remove(SelectedAccess);
            SelectedAccess = null;
        }

        private async Task EditAccountCommandHandler()
        {
            var results = _accountInputValidator.Validate(AccountInput);

            if (!results.IsValid)
            {
                _dialogService.ShowMessageBox("Error", results.ToString());
                return;
            }
            var mapper = new AccountInputMapper();
            var dto = mapper.Map(AccountInput);

            await _mediator.Send(new EditAccountCommand(dto));
            _dialogService.ShowMessageBox("Information", "Edited accounts");
        }

        private async Task LoadAccount()
        {
            var account = await _mediator.Send(new GetAccountByIdQuery(AccountId));

            await Observable.Start(() =>
            {
                AccountInput.Id = account.Id;
                AccountInput.Username = account.Username;
                AccountInput.Server = account.Server;
                AccountInput.SetAccesses(account.Accesses);

                AccessInput.Clear();
            }, RxApp.MainThreadScheduler);
        }

        public AccessDto SelectedAccess
        {
            get => _selectedAcess;
            set => this.RaiseAndSetIfChanged(ref _selectedAcess, value);
        }
    }
}