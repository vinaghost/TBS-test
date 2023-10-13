using FluentValidation;
using MainCore.Common.Repositories;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.UI.Models.Input;
using MainCore.UI.ViewModels.Abstract;
using MainCore.UI.ViewModels.UserControls;
using ReactiveUI;
using System.Reactive;

namespace MainCore.UI.ViewModels.Tabs.Villages
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class VillageSettingViewModel : VillageTabViewModelBase
    {
        public VillageSettingInput VillageSettingInput { get; } = new();
        private readonly IValidator<VillageSettingInput> _villageSettingInputValidator;

        private readonly IVillageSettingRepository _villageSettingRepository;

        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }

        public VillageSettingViewModel(IVillageSettingRepository villageSettingRepository, WaitingOverlayViewModel waitingOverlayViewModel, IValidator<VillageSettingInput> villageSettingInputValidator)
        {
            _villageSettingRepository = villageSettingRepository;

            _waitingOverlayViewModel = waitingOverlayViewModel;
            _villageSettingInputValidator = villageSettingInputValidator;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            ExportCommand = ReactiveCommand.CreateFromTask(ExportTask);
            ImportCommand = ReactiveCommand.CreateFromTask(ImportTask);
        }

        protected override async Task Load(int villageId)
        {
            var settings = await _villageSettingRepository.Get(villageId);
            VillageSettingInput.Set(settings);
        }

        private async Task Save(int villageId)
        {
            var settings = VillageSettingInput.Get();
            await _villageSettingRepository.Set(villageId, settings);
        }

        private async Task SaveTask()
        {
            var result = _villageSettingInputValidator.Validate(VillageSettingInput);
            if (!result.IsValid)
            {
                //_messageService.Show("Error", result.ToString());
            }
            else
            {
                _waitingOverlayViewModel.Show("saving settings ...");
                await Save(VillageId);
                _waitingOverlayViewModel.Close();
                //_messageService.Show("Information", "Settings saved");
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
            //Dictionary<VillageSettingEnums, int> settings;
            //try
            //{
            //    var jsonString = await File.ReadAllTextAsync(path);
            //    settings = JsonSerializer.Deserialize<Dictionary<VillageSettingEnums, int>>(jsonString);
            //}
            //catch
            //{
            //    //_messageService.Show("Warning", "Invalid file.");
            //    return;
            //}

            //VillageSettingInput.Set(settings);
            //var result = _villageSettingInputValidator.Validate(VillageSettingInput);
            //if (!result.IsValid)
            //{
            //    //_messageService.Show(title: "Error", result.ToString());
            //    return;
            //}
            //else
            //{
            //    _waitingOverlayViewModel.Show("importing settings ...");
            //    await _villageSettingRepository.Set(VillageId, settings);
            //    //_messageService.Show("Information", "Settings imported");
            //    _waitingOverlayViewModel.Close();
            //}
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
            //var settings = await _villageSettingRepository.Get(VillageId);
            //var jsonString = await Task.Run(() => JsonSerializer.Serialize(settings));
            //await File.WriteAllTextAsync(path, jsonString);
            //_waitingOverlayViewModel.Close();

            //await _dialogService.ShowMessageBoxAsync(this,
            //    "Settings exported",
            //    "Information");
        }
    }
}