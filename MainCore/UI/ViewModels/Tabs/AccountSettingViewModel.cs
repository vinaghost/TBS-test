using FluentValidation;
using MainCore.Common.Enums;
using MainCore.Common.Repositories;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.UI.Models.Input;
using MainCore.UI.ViewModels.Abstract;
using MainCore.UI.ViewModels.UserControls;
using ReactiveUI;
using System.Reactive;
using System.Text.Json;

namespace MainCore.UI.ViewModels.Tabs
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class AccountSettingViewModel : AccountTabViewModelBase
    {
        public AccountSettingInput AccountSettingInput { get; } = new();
        private readonly IValidator<AccountSettingInput> _accountsettingInputValidator;

        private readonly IAccountSettingRepository _accountSettingRepository;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;
        private readonly IDialogService _dialogService;
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }

        public AccountSettingViewModel(IAccountSettingRepository accountSettingRepository, WaitingOverlayViewModel waitingOverlayViewModel, IValidator<AccountSettingInput> accountsettingInputValidator, IDialogService dialogService)
        {
            _accountSettingRepository = accountSettingRepository;
            _waitingOverlayViewModel = waitingOverlayViewModel;
            _accountsettingInputValidator = accountsettingInputValidator;
            _dialogService = dialogService;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            ExportCommand = ReactiveCommand.CreateFromTask(ExportTask);
            ImportCommand = ReactiveCommand.CreateFromTask(ImportTask);
        }

        protected override async Task Load(AccountId accountId)
        {
            var settings = await Task.Run(() => _accountSettingRepository.Get(accountId));
            AccountSettingInput.Set(settings);
        }

        private async Task Save(AccountId accountId)
        {
            var settings = AccountSettingInput.Get();
            await Task.Run(() => _accountSettingRepository.Set(accountId, settings));
        }

        private async Task SaveTask()
        {
            var result = _accountsettingInputValidator.Validate(AccountSettingInput);
            if (!result.IsValid)
            {
                _dialogService.ShowMessageBox("Error", result.ToString());
            }
            else
            {
                await _waitingOverlayViewModel.Show(
                     "saving settings ...",
                     () => Save(AccountId)
                 );

                _dialogService.ShowMessageBox("Information", "Settings saved");
            }
        }

        private async Task ImportTask()
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
            else
            {
                _accountSettingRepository.Set(AccountId, settings);
                _dialogService.ShowMessageBox("Information", "Settings imported");
            }
        }

        private async Task ExportTask()
        {
            var path = _dialogService.SaveFileDialog();
            await _waitingOverlayViewModel.Show(
                "exporting settings ...",
                async () =>
                {
                    var settings = await Task.Run(() => _accountSettingRepository.Get(AccountId));
                    var jsonString = await Task.Run(() => JsonSerializer.Serialize(settings));
                    await File.WriteAllTextAsync(path, jsonString);
                });

            _dialogService.ShowMessageBox("Settings exported", "Information");
        }
    }
}