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

namespace MainCore.UI.ViewModels.Tabs.Villages
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class VillageSettingViewModel : VillageTabViewModelBase
    {
        public VillageSettingInput VillageSettingInput { get; } = new();
        private readonly IValidator<VillageSettingInput> _villageSettingInputValidator;

        private readonly IVillageSettingRepository _villageSettingRepository;

        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;
        private readonly MessageBoxViewModel _messageBoxViewModel;
        private readonly FileDialogViewModel _fileDialogViewModel;

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }

        public VillageSettingViewModel(IVillageSettingRepository villageSettingRepository, WaitingOverlayViewModel waitingOverlayViewModel, IValidator<VillageSettingInput> villageSettingInputValidator, MessageBoxViewModel messageBoxViewModel, FileDialogViewModel fileDialogViewModel)
        {
            _villageSettingRepository = villageSettingRepository;

            _waitingOverlayViewModel = waitingOverlayViewModel;
            _villageSettingInputValidator = villageSettingInputValidator;
            _messageBoxViewModel = messageBoxViewModel;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            ExportCommand = ReactiveCommand.CreateFromTask(ExportTask);
            ImportCommand = ReactiveCommand.CreateFromTask(ImportTask);
            _fileDialogViewModel = fileDialogViewModel;
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
                await _messageBoxViewModel.Show("Error", result.ToString());
            }
            else
            {
                _waitingOverlayViewModel.Show("saving settings ...");
                await Save(VillageId);
                _waitingOverlayViewModel.Close();
                await _messageBoxViewModel.Show("Information", "Settings saved");
            }
        }

        private async Task ImportTask()
        {
            var path = _fileDialogViewModel.OpenFileDialog();
            Dictionary<VillageSettingEnums, int> settings;
            try
            {
                var jsonString = await File.ReadAllTextAsync(path);
                settings = JsonSerializer.Deserialize<Dictionary<VillageSettingEnums, int>>(jsonString);
            }
            catch
            {
                await _messageBoxViewModel.Show("Warning", "Invalid file.");
                return;
            }

            VillageSettingInput.Set(settings);
            var result = _villageSettingInputValidator.Validate(VillageSettingInput);
            if (!result.IsValid)
            {
                await _messageBoxViewModel.Show("Error", result.ToString());
                return;
            }
            else
            {
                _waitingOverlayViewModel.Show("importing settings ...");
                await _villageSettingRepository.Set(VillageId, settings);
                _waitingOverlayViewModel.Close();
                await _messageBoxViewModel.Show("Information", "Settings imported");
            }
        }

        private async Task ExportTask()
        {
            var path = _fileDialogViewModel.SaveFileDialog();

            _waitingOverlayViewModel.Show("exporting settings ...");
            var settings = await _villageSettingRepository.Get(VillageId);
            var jsonString = await Task.Run(() => JsonSerializer.Serialize(settings));
            await File.WriteAllTextAsync(path, jsonString);
            _waitingOverlayViewModel.Close();
            await _messageBoxViewModel.Show("Information", "Settings exported");
        }
    }
}