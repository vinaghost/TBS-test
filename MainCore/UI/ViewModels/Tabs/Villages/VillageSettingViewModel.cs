using FluentValidation;
using MainCore.Common.Enums;
using MainCore.Repositories;
using MainCore.CQRS.Commands;
using MainCore.CQRS.Queries;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.UI.Models.Input;
using MainCore.UI.ViewModels.Abstract;
using MainCore.UI.ViewModels.UserControls;
using MediatR;
using ReactiveUI;
using System.Reactive.Linq;
using System.Text.Json;
using Unit = System.Reactive.Unit;

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
        private readonly IMediator _mediator;

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }

        public VillageSettingViewModel(IVillageSettingRepository villageSettingRepository, WaitingOverlayViewModel waitingOverlayViewModel, IValidator<VillageSettingInput> villageSettingInputValidator, IDialogService dialogService, IMediator mediator)
        {
            _villageSettingRepository = villageSettingRepository;

            _waitingOverlayViewModel = waitingOverlayViewModel;
            _villageSettingInputValidator = villageSettingInputValidator;
            _dialogService = dialogService;
            _mediator = mediator;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveCommandHandler);
            ExportCommand = ReactiveCommand.CreateFromTask(ExportCommandHandler);
            ImportCommand = ReactiveCommand.CreateFromTask(ImportCommandHandler);
        }

        public async Task SettingRefresh(VillageId villageId)
        {
            if (!IsActive) return;
            if (villageId != VillageId) return;
            await LoadSettings(villageId);
        }

        protected override async Task Load(VillageId villageId)
        {
            await LoadSettings(villageId);
        }

        private async Task SaveCommandHandler()
        {
            var result = _villageSettingInputValidator.Validate(VillageSettingInput);
            if (!result.IsValid)
            {
                _dialogService.ShowMessageBox("Error", result.ToString());
                return;
            }
            var settings = VillageSettingInput.Get();
            await _mediator.Send(new SaveVillageSettingByIdCommand(VillageId, settings));
            _dialogService.ShowMessageBox("Information", "Settings saved");
        }

        private async Task ImportCommandHandler()
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
            await _mediator.Send(new SaveVillageSettingByIdCommand(VillageId, VillageSettingInput.Get()));
            _dialogService.ShowMessageBox("Information", "Settings imported");
        }

        private async Task ExportCommandHandler()
        {
            var path = _dialogService.SaveFileDialog();
            var settings = await _mediator.Send(new GetVillageSettingDictonaryByIdQuery(VillageId));
            var jsonString = JsonSerializer.Serialize(settings);
            await File.WriteAllTextAsync(path, jsonString);
            _dialogService.ShowMessageBox("Settings exported", "Information");
        }

        private async Task LoadSettings(VillageId villageId)
        {
            var settings = await _mediator.Send(new GetVillageSettingDictonaryByIdQuery(villageId));
            await Observable.Start(() =>
            {
                VillageSettingInput.Set(settings);
            }, RxApp.MainThreadScheduler);
        }
    }
}