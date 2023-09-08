using FluentValidation;
using MainCore.Enums;
using Microsoft.Win32;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Text.Json;
using System.Threading.Tasks;
using WPFUI.Models.Input;
using WPFUI.Repositories;
using WPFUI.Services;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.UserControls;

namespace WPFUI.ViewModels.Tabs
{
    public class AccountSettingViewModel : AccountTabViewModelBase
    {
        public AccountSettingInput AccountSettingInput { get; } = new();
        private readonly IValidator<AccountSettingInput> _accountsettingInputValidator;

        private readonly IAccountSettingRepository _accountSettingRepository;
        private readonly IMessageService _messageService;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }

        public AccountSettingViewModel(IAccountSettingRepository accountSettingRepository, IMessageService messageService, WaitingOverlayViewModel waitingOverlayViewModel, IValidator<AccountSettingInput> accountsettingInputValidator)
        {
            _accountSettingRepository = accountSettingRepository;
            _messageService = messageService;
            _waitingOverlayViewModel = waitingOverlayViewModel;
            _accountsettingInputValidator = accountsettingInputValidator;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            ExportCommand = ReactiveCommand.CreateFromTask(ExportTask);
            ImportCommand = ReactiveCommand.CreateFromTask(ImportTask);
        }

        protected override async Task Load(int accountId)
        {
            await _accountSettingRepository.CheckSetting(accountId);
            var settings = await _accountSettingRepository.Get(accountId);
            AccountSettingInput.Set(settings);
        }

        private async Task Save(int accountId)
        {
            var settings = AccountSettingInput.Get();
            await _accountSettingRepository.Set(accountId, settings);
        }

        private async Task SaveTask()
        {
            var result = _accountsettingInputValidator.Validate(AccountSettingInput);
            if (!result.IsValid)
            {
                _messageService.Show("Error", result.ToString());
            }
            else
            {
                _waitingOverlayViewModel.Show("saving settings ...");
                await Save(AccountId);
                _waitingOverlayViewModel.Close();
                _messageService.Show("Information", "Settings saved");
            }
        }

        private async Task ImportTask()
        {
            var ofd = new OpenFileDialog
            {
                InitialDirectory = AppContext.BaseDirectory,
                Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = $"{AccountId}_settings.tbs",
            };

            if (ofd.ShowDialog() == true)
            {
                Dictionary<AccountSettingEnums, int> settings;
                try
                {
                    var jsonString = await File.ReadAllTextAsync(ofd.FileName);
                    settings = JsonSerializer.Deserialize<Dictionary<AccountSettingEnums, int>>(jsonString);
                }
                catch
                {
                    _messageService.Show("Warning", "Invalid file.");
                    return;
                }

                AccountSettingInput.Set(settings);
                var result = _accountsettingInputValidator.Validate(AccountSettingInput);
                if (!result.IsValid)
                {
                    _messageService.Show(title: "Error", result.ToString());
                    return;
                }
                else
                {
                    _waitingOverlayViewModel.Show("importing settings ...");
                    await _accountSettingRepository.Set(AccountId, settings);
                    _messageService.Show("Information", "Settings imported");
                    _waitingOverlayViewModel.Close();
                }
            }
        }

        private async Task ExportTask()
        {
            var svd = new SaveFileDialog
            {
                InitialDirectory = AppContext.BaseDirectory,
                Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = $"{AccountId}_settings.tbs",
            };

            if (svd.ShowDialog() == true)
            {
                _waitingOverlayViewModel.Show("exporting settings ...");
                var settings = await _accountSettingRepository.Get(AccountId);
                var jsonString = await Task.Run(() => JsonSerializer.Serialize(settings));
                await File.WriteAllTextAsync(svd.FileName, jsonString);
                _waitingOverlayViewModel.Close();
                _messageService.Show("Information", "Settings exported");
            }
        }
    }
}