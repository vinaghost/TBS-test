﻿using FluentValidation;
using MainCore.Common.Repositories;
using MainCore.Features.Farming.Tasks;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.UI.Models.Input;
using MainCore.UI.Models.Output;
using MainCore.UI.ViewModels.Abstract;
using MainCore.UI.ViewModels.UserControls;
using ReactiveUI;
using System.Drawing;
using System.Reactive;
using System.Reactive.Linq;

namespace MainCore.UI.ViewModels.Tabs
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class FarmingViewModel : AccountTabViewModelBase
    {
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;
        private readonly IFarmListRepository _farmListRepository;
        private readonly IAccountSettingRepository _accountSettingRepository;
        public FarmListSettingInput FarmListSettingInput { get; } = new();
        private readonly IValidator<FarmListSettingInput> _farmListSettingInputValidator;
        public ListBoxItemViewModel FarmLists { get; } = new();

        private readonly ITaskManager _taskManager;
        private readonly MessageBoxViewModel _messageBoxViewModel;

        public FarmingViewModel(IFarmListRepository farmListRepository, IAccountSettingRepository accountSettingRepository, IValidator<FarmListSettingInput> farmListSettingInputValidator, WaitingOverlayViewModel waitingOverlayViewModel, ITaskManager taskManager, MessageBoxViewModel messageBoxViewModel)

        {
            _farmListRepository = farmListRepository;
            _accountSettingRepository = accountSettingRepository;
            _farmListSettingInputValidator = farmListSettingInputValidator;

            _waitingOverlayViewModel = waitingOverlayViewModel;
            _taskManager = taskManager;
            _messageBoxViewModel = messageBoxViewModel;

            LoadFarmListCommand = ReactiveCommand.CreateFromTask(AddLoadTask);
            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            ActiveFarmListCommand = ReactiveCommand.CreateFromTask(ActiveFarmListTask);
            StartCommand = ReactiveCommand.CreateFromTask(StartTask);
            StopCommand = ReactiveCommand.CreateFromTask(StopTask);
        }

        public async Task FarmListsUpdated(int accountId)
        {
            if (!IsActive) return;
            if (accountId != AccountId) return;
            await Observable.Start(
                async () => await LoadFarmLists(accountId),
                RxApp.MainThreadScheduler);
        }

        protected override async Task Load(int accountId)
        {
            await LoadFarmLists(accountId);
            await LoadSettings(accountId);
        }

        private async Task AddLoadTask()
        {
            _taskManager.AddOrUpdate<UpdateFarmListTask>(AccountId);
            await _messageBoxViewModel.Show("Information", "Added update farm list task");
        }

        private async Task StartTask()
        {
            if (!FarmListSettingInput.UseStartAllButton)
            {
                var count = await Task.Run(() => _farmListRepository.CountActiveFarmLists(AccountId));
                if (count == 0)
                {
                    await _messageBoxViewModel.Show("Information", "There is no active farm or use start all button is disable");
                    return;
                }
            }
            var task = await Task.Run(() => _taskManager.Get<StartFarmListTask>(AccountId));
            if (task is null)
            {
                _taskManager.AddOrUpdate<StartFarmListTask>(AccountId);
            }
            await _messageBoxViewModel.Show("Information", "Added start farm list task");
        }

        private async Task StopTask()
        {
            var task = await Task.Run(() => _taskManager.Get<StartFarmListTask>(AccountId));

            if (task is not null)
            {
                _taskManager.Remove(AccountId, task);
            }
            await _messageBoxViewModel.Show("Information", "Removed start farm list task");
        }

        private async Task Save(int accountId)
        {
            var settings = FarmListSettingInput.Get();
            await Task.Run(() => _accountSettingRepository.Set(accountId, settings));
        }

        private async Task SaveTask()
        {
            var result = _farmListSettingInputValidator.Validate(FarmListSettingInput);
            if (!result.IsValid)
            {
                await _messageBoxViewModel.Show("Error", result.ToString());
            }
            else
            {
                await _waitingOverlayViewModel.Show(
                    "saving settings ...",
                    () => Save(AccountId));
                await _messageBoxViewModel.Show("Information", "Settings saved");
            }
        }

        private async Task ActiveFarmListTask()
        {
            var SelectedFarmList = FarmLists.SelectedItem;
            if (FarmLists.SelectedItem is null)
            {
                await _messageBoxViewModel.Show("Warning", "No farm list selected");
                return;
            }
            await Task.Run(() => _farmListRepository.ActiveFarmList(SelectedFarmList.Id));
            if (SelectedFarmList.Color == Color.Green)
            {
                SelectedFarmList.Color = Color.Red;
            }
            else
            {
                SelectedFarmList.Color = Color.Green;
            }
        }

        private async Task LoadSettings(int accountId)
        {
            var settings = await Task.Run(() => _accountSettingRepository.Get(accountId));
            FarmListSettingInput.Set(settings);
        }

        private async Task LoadFarmLists(int accountId)
        {
            var farmLists = await Task.Run(() => _farmListRepository.GetList(accountId));
            FarmLists.Load(farmLists.Select(x => new ListBoxItem(x)));
        }

        public ReactiveCommand<Unit, Unit> LoadFarmListCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ActiveFarmListCommand { get; }
        public ReactiveCommand<Unit, Unit> StartCommand { get; }
        public ReactiveCommand<Unit, Unit> StopCommand { get; }
    }
}