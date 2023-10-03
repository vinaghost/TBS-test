using FluentValidation;
using MainCore.Common.Enums;
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

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class VillageSettingViewModel : VillageTabViewModelBase
    {
        public VillageSettingInput VillageSettingInput { get; } = new();
        private readonly IValidator<VillageSettingInput> _villageSettingInputValidator;

        private readonly IVillageSettingRepository _villageSettingRepository;
        private readonly IMessageService _messageService;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }

        public VillageSettingViewModel(IVillageSettingRepository villageSettingRepository, IMessageService messageService, WaitingOverlayViewModel waitingOverlayViewModel, IValidator<VillageSettingInput> villageSettingInputValidator)
        {
            _villageSettingRepository = villageSettingRepository;
            _messageService = messageService;
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
                _messageService.Show("Error", result.ToString());
            }
            else
            {
                _waitingOverlayViewModel.Show("saving settings ...");
                await Save(VillageId);
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
                FileName = $"{VillageId}_settings.tbs",
            };

            if (ofd.ShowDialog() == true)
            {
                Dictionary<VillageSettingEnums, int> settings;
                try
                {
                    var jsonString = await File.ReadAllTextAsync(ofd.FileName);
                    settings = JsonSerializer.Deserialize<Dictionary<VillageSettingEnums, int>>(jsonString);
                }
                catch
                {
                    _messageService.Show("Warning", "Invalid file.");
                    return;
                }

                VillageSettingInput.Set(settings);
                var result = _villageSettingInputValidator.Validate(VillageSettingInput);
                if (!result.IsValid)
                {
                    _messageService.Show(title: "Error", result.ToString());
                    return;
                }
                else
                {
                    _waitingOverlayViewModel.Show("importing settings ...");
                    await _villageSettingRepository.Set(VillageId, settings);
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
                FileName = $"{VillageId}_settings.tbs",
            };

            if (svd.ShowDialog() == true)
            {
                _waitingOverlayViewModel.Show("exporting settings ...");
                var settings = await _villageSettingRepository.Get(VillageId);
                var jsonString = await Task.Run(() => JsonSerializer.Serialize(settings));
                await File.WriteAllTextAsync(svd.FileName, jsonString);
                _waitingOverlayViewModel.Close();
                _messageService.Show("Information", "Settings exported");
            }
        }
    }
}