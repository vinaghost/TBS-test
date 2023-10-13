using FluentValidation;
using MainCore.Common.Repositories;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.UI.Models.Input;
using MainCore.UI.ViewModels.Abstract;
using MainCore.UI.ViewModels.UserControls;
using ReactiveUI;
using System.Reactive;

namespace MainCore.UI.ViewModels.Tabs
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class AccountSettingViewModel : AccountTabViewModelBase
    {
        public AccountSettingInput AccountSettingInput { get; } = new();
        private readonly IValidator<AccountSettingInput> _accountsettingInputValidator;

        private readonly IAccountSettingRepository _accountSettingRepository;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }

        public AccountSettingViewModel(IAccountSettingRepository accountSettingRepository, WaitingOverlayViewModel waitingOverlayViewModel, IValidator<AccountSettingInput> accountsettingInputValidator)
        {
            _accountSettingRepository = accountSettingRepository;
            _waitingOverlayViewModel = waitingOverlayViewModel;
            _accountsettingInputValidator = accountsettingInputValidator;

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
                ////_messageService.Show("Error", result.ToString());
            }
            else
            {
                _waitingOverlayViewModel.Show("saving settings ...");
                await Save(AccountId);
                _waitingOverlayViewModel.Close();
                ////_messageService.Show("Information", "Settings saved");
            }
        }

        private async Task ImportTask()
        {
            //var openFileDialogSettings = new OpenFileDialogSettings
            //{
            //    Title = "Import settings",
            //    InitialDirectory = AppContext.BaseDirectory,
            //    Filters = new List<FileFilter>()
            //    {
            //        new FileFilter("TBS files", "tbs"),
            //        new FileFilter("All Files", "*")
            //    },
            //};

            //var resultOfd = await _dialogService.ShowOpenFileDialogAsync(this, openFileDialogSettings);
            //if (resultOfd is null) return;
            //var path = resultOfd.LocalPath ?? "";
            //    Dictionary<AccountSettingEnums, int> settings;
            //    try
            //    {
            //        var jsonString = await File.ReadAllTextAsync(path);
            //        settings = JsonSerializer.Deserialize<Dictionary<AccountSettingEnums, int>>(jsonString);
            //    }
            //    catch
            //    {
            //        //_messageService.Show("Warning", "Invalid file.");
            //        return;
            //    }

            //    AccountSettingInput.Set(settings);
            //    var result = _accountsettingInputValidator.Validate(AccountSettingInput);
            //    if (!result.IsValid)
            //    {
            //        //_messageService.Show(title: "Error", result.ToString());
            //        return;
            //    }
            //    else
            //    {
            //        _waitingOverlayViewModel.Show("importing settings ...");
            //        await _accountSettingRepository.Set(AccountId, settings);
            //        //_messageService.Show("Information", "Settings imported");
            //        _waitingOverlayViewModel.Close();
            //    }
        }

        private async Task ExportTask()
        {
            //var saveFileDialogSettings = new SaveFileDialogSettings
            //{
            //    Title = "Export settings",
            //    InitialDirectory = AppContext.BaseDirectory,
            //    Filters = new List<FileFilter>()
            //    {
            //        new FileFilter("TBS files", "tbs"),
            //        new FileFilter("All Files", "*")
            //    }
            //};
            //var resultOfd = await _dialogService.ShowSaveFileDialogAsync(this, saveFileDialogSettings);
            //if (resultOfd is null) return;
            //var path = resultOfd.LocalPath ?? "";
            //_waitingOverlayViewModel.Show("exporting settings ...");
            //var settings = await _accountSettingRepository.Get(AccountId);
            //var jsonString = await Task.Run(() => JsonSerializer.Serialize(settings));
            //await File.WriteAllTextAsync(path, jsonString);
            //_waitingOverlayViewModel.Close();

            //await _dialogService.ShowMessageBoxAsync(this,
            //    "Settings exported",
            //    "Information");
        }
    }
}