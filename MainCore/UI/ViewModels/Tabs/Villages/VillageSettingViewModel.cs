using FluentValidation;
using MainCore.Common.Enums;
using MainCore.Common.Repositories;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.UI.Models.Input;
using MainCore.UI.ViewModels.Abstract;
using MainCore.UI.ViewModels.UserControls;
using ReactiveUI;
using System.Reactive;
using System.Text.Json;

namespace MainCore.UI.ViewModels.Tabs.Villages
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class VillageSettingViewModel : VillageTabViewModelBase
    {
        public VillageSettingInput VillageSettingInput { get; } = new();
        private readonly IValidator<VillageSettingInput> _villageSettingInputValidator;

        private readonly IVillageSettingRepository _villageSettingRepository;

        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;
        private readonly IDialogService _dialogService;

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }

        public VillageSettingViewModel(IVillageSettingRepository villageSettingRepository, WaitingOverlayViewModel waitingOverlayViewModel, IValidator<VillageSettingInput> villageSettingInputValidator, IDialogService dialogService)
        {
            _villageSettingRepository = villageSettingRepository;

            _waitingOverlayViewModel = waitingOverlayViewModel;
            _villageSettingInputValidator = villageSettingInputValidator;
            _dialogService = dialogService;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            ExportCommand = ReactiveCommand.CreateFromTask(ExportTask);
            ImportCommand = ReactiveCommand.CreateFromTask(ImportTask);
        }

        protected override async Task Load(int villageId)
        {
            var settings = await Task.Run(() => _villageSettingRepository.Get(villageId));
            VillageSettingInput.Set(settings);
        }

        private async Task Save(int villageId)
        {
            var settings = VillageSettingInput.Get();
            await Task.Run(() => _villageSettingRepository.Set(villageId, settings));
        }

        private async Task SaveTask()
        {
            var result = _villageSettingInputValidator.Validate(VillageSettingInput);
            if (!result.IsValid)
            {
                _dialogService.ShowMessageBox("Error", result.ToString());
            }
            else
            {
                await _waitingOverlayViewModel.Show(
                    "saving settings ...",
                    () => Save(VillageId)
                );
                _dialogService.ShowMessageBox("Information", "Settings saved");
            }
        }

        private async Task ImportTask()
        {
            var path = _dialogService.OpenFileDialog();
            Dictionary<VillageSettingEnums, int> settings;
            try
            {
                var jsonString = await File.ReadAllTextAsync(path);
                settings = JsonSerializer.Deserialize<Dictionary<VillageSettingEnums, int>>(jsonString);
            }
            catch
            {
                _dialogService.ShowMessageBox("Warning", "Invalid file.");
                return;
            }

            VillageSettingInput.Set(settings);
            var result = _villageSettingInputValidator.Validate(VillageSettingInput);
            if (!result.IsValid)
            {
                _dialogService.ShowMessageBox("Error", result.ToString());
                return;
            }
            else
            {
                _villageSettingRepository.Set(VillageId, settings);
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
                    var settings = await Task.Run(() => _villageSettingRepository.Get(VillageId));
                    var jsonString = await Task.Run(() => JsonSerializer.Serialize(settings));
                    await File.WriteAllTextAsync(path, jsonString);
                });

            _dialogService.ShowMessageBox("Information", "Settings exported");
        }
    }
}