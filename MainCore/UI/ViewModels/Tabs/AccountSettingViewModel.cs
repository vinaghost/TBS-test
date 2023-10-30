﻿using FluentValidation;
using MainCore.Common.Enums;
using MainCore.CQRS.Commands;
using MainCore.CQRS.Queries;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.UI.Models.Input;
using MainCore.UI.ViewModels.Abstract;
using MediatR;
using ReactiveUI;
using System.Reactive.Linq;
using System.Text.Json;
using Unit = System.Reactive.Unit;

namespace MainCore.UI.ViewModels.Tabs
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class AccountSettingViewModel : AccountTabViewModelBase
    {
        public AccountSettingInput AccountSettingInput { get; } = new();
        private readonly IValidator<AccountSettingInput> _accountsettingInputValidator;

        private readonly IDialogService _dialogService;
        private readonly IMediator _mediator;
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }

        public AccountSettingViewModel(IValidator<AccountSettingInput> accountsettingInputValidator, IDialogService dialogService, IMediator mediator)
        {
            _accountsettingInputValidator = accountsettingInputValidator;
            _dialogService = dialogService;
            _mediator = mediator;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveCommandHandler);
            ExportCommand = ReactiveCommand.CreateFromTask(ExportCommandHandler);
            ImportCommand = ReactiveCommand.CreateFromTask(ImportCommandHandler);
        }

        public async Task SettingRefresh(AccountId accountId)
        {
            if (!IsActive) return;
            if (accountId != AccountId) return;
            await LoadSettings(accountId);
        }

        protected override async Task Load(AccountId accountId)
        {
            await LoadSettings(accountId);
        }

        private async Task SaveCommandHandler()
        {
            var result = _accountsettingInputValidator.Validate(AccountSettingInput);
            if (!result.IsValid)
            {
                _dialogService.ShowMessageBox("Error", result.ToString());
                return;
            }
            var settings = AccountSettingInput.Get();
            await _mediator.Send(new SaveAccountSettingByIdCommand(AccountId, settings));
            _dialogService.ShowMessageBox("Information", "Settings saved");
        }

        private async Task ImportCommandHandler()
        {
            var path = _dialogService.OpenFileDialog();
            Dictionary<AccountSettingEnums, int> settings;
            try
            {
                var jsonString = await File.ReadAllTextAsync(path);
                settings = JsonSerializer.Deserialize<Dictionary<AccountSettingEnums, int>>(jsonString);
            }
            catch
            {
                _dialogService.ShowMessageBox("Warning", "Invalid file.");
                return;
            }

            AccountSettingInput.Set(settings);
            var result = _accountsettingInputValidator.Validate(AccountSettingInput);
            if (!result.IsValid)
            {
                _dialogService.ShowMessageBox("Error", result.ToString());
                return;
            }
            await _mediator.Send(new SaveAccountSettingByIdCommand(AccountId, AccountSettingInput.Get()));
            _dialogService.ShowMessageBox("Information", "Settings imported");
        }

        private async Task ExportCommandHandler()
        {
            var path = _dialogService.SaveFileDialog();
            var settings = await _mediator.Send(new GetAccountSettingDictonaryByIdQuery(AccountId));
            var jsonString = JsonSerializer.Serialize(settings);
            await File.WriteAllTextAsync(path, jsonString);
            _dialogService.ShowMessageBox("Settings exported", "Information");
        }

        private async Task LoadSettings(AccountId accountId)
        {
            var settings = await _mediator.Send(new GetAccountSettingDictonaryByIdQuery(accountId));
            await Observable.Start(() =>
            {
                AccountSettingInput.Set(settings);
            }, RxApp.MainThreadScheduler);
        }
    }
}