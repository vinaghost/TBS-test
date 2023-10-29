using FluentValidation;
using MainCore.CQRS.Commands;
using MainCore.DTO;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.UI.Models.Input;
using MainCore.UI.ViewModels.Abstract;
using MediatR;
using ReactiveUI;
using Unit = System.Reactive.Unit;

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
        private readonly IDialogService _dialogService;
        private readonly IMediator _mediator;
        public ReactiveCommand<Unit, Unit> AddAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> EditAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> AddAccountCommand { get; }

        public AddAccountViewModel(IValidator<AccessInput> accessInputValidator, IValidator<AccountInput> accountInputValidator, IDialogService dialogService, IMediator mediator)
        {
            _mediator = mediator;

            _accessInputValidator = accessInputValidator;
            _accountInputValidator = accountInputValidator;
            _dialogService = dialogService;

            AddAccessCommand = ReactiveCommand.Create(AddAccessCommandHandler);
            EditAccessCommand = ReactiveCommand.Create(EditAccessCommandHandler);
            DeleteAccessCommand = ReactiveCommand.Create(DeleteAccessCommandHandler);
            AddAccountCommand = ReactiveCommand.CreateFromTask(AddAccountCommandHandler);
            this.WhenAnyValue(vm => vm.SelectedAccess)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    var mapper = new AccessInputMapper();
                    mapper.Map(x, AccessInput);
                });
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

        private async Task AddAccountCommandHandler()
        {
            var results = _accountInputValidator.Validate(AccountInput);

            if (!results.IsValid)
            {
                _dialogService.ShowMessageBox("Error", results.ToString());
                return;
            }
            var mapper = new AccountInputMapper();
            var dto = mapper.Map(AccountInput);
            await _mediator.Send(new AddAccountCommand(dto));
            _dialogService.ShowMessageBox("Information", "Added account");
        }

        public AccessDto SelectedAccess
        {
            get => _selectedAccess;
            set => this.RaiseAndSetIfChanged(ref _selectedAccess, value);
        }
    }
}