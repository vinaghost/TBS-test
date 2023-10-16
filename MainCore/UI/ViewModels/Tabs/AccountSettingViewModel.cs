using FluentValidation;
using MainCore.Common.Enums;
using MainCore.Common.Repositories;
using MainCore.Infrasturecture.AutoRegisterDi;
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
        private readonly MessageBoxViewModel _messageBoxViewModel;
        private readonly FileDialogViewModel _fileDialogViewModel;
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }

        public AccountSettingViewModel(IAccountSettingRepository accountSettingRepository, WaitingOverlayViewModel waitingOverlayViewModel, IValidator<AccountSettingInput> accountsettingInputValidator, MessageBoxViewModel messageBoxViewModel, FileDialogViewModel fileDialogViewModel)
        {
            _accountSettingRepository = accountSettingRepository;
            _waitingOverlayViewModel = waitingOverlayViewModel;
            _accountsettingInputValidator = accountsettingInputValidator;
            _messageBoxViewModel = messageBoxViewModel;
            _fileDialogViewModel = fileDialogViewModel;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            ExportCommand = ReactiveCommand.CreateFromTask(ExportTask);
            ImportCommand = ReactiveCommand.CreateFromTask(ImportTask);
        }

        protected override async Task Load(int accountId)
        {
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
                await _messageBoxViewModel.Show("Error", result.ToString());
            }
            else
            {
                await _waitingOverlayViewModel.Show(
                     "saving settings ...",
                     () => Save(AccountId)
                 );

                await _messageBoxViewModel.Show("Information", "Settings saved");
            }
        }

        private async Task ImportTask()
        {
            var path = _fileDialogViewModel.OpenFileDialog();
            Dictionary<AccountSettingEnums, int> settings;
            try
            {
                var jsonString = await File.ReadAllTextAsync(path);
                settings = JsonSerializer.Deserialize<Dictionary<AccountSettingEnums, int>>(jsonString);
            }
            catch
            {
                await _messageBoxViewModel.Show("Warning", "Invalid file.");
                return;
            }

            AccountSettingInput.Set(settings);
            var result = _accountsettingInputValidator.Validate(AccountSettingInput);
            if (!result.IsValid)
            {
                await _messageBoxViewModel.Show("Error", result.ToString());
                return;
            }
            else
            {
                await _waitingOverlayViewModel.Show(
                    "importing settings ...",
                    () => _accountSettingRepository.Set(AccountId, settings));
                await _messageBoxViewModel.Show("Information", "Settings imported");
            }
        }

        private async Task ExportTask()
        {
            var path = _fileDialogViewModel.SaveFileDialog();
            await _waitingOverlayViewModel.Show(
                "exporting settings ...",
                async () =>
                {
                    var settings = await _accountSettingRepository.Get(AccountId);
                    var jsonString = await Task.Run(() => JsonSerializer.Serialize(settings));
                    await File.WriteAllTextAsync(path, jsonString);
                });

            await _messageBoxViewModel.Show("Settings exported", "Information");
        }
    }
}