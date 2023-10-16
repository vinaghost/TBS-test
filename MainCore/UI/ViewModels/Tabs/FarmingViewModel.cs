using FluentValidation;
using MainCore.Common.Repositories;
using MainCore.Features.Farming.Tasks;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.UI.Models.Input;
using MainCore.UI.Models.Output;
using MainCore.UI.ViewModels.Abstract;
using MainCore.UI.ViewModels.UserControls;
using ReactiveUI;
using System.Collections.ObjectModel;
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
        public FarmListSettingInput FarmListSettingInput { get; set; } = new();
        private readonly IValidator<FarmListSettingInput> _farmListSettingInputValidator;

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

            _farmListRepository.FarmListsUpdated += FarmListsUpdated;
        }

        private async Task FarmListsUpdated(int accountId)
        {
            if (!IsActive) return;
            if (accountId != AccountId) return;
            await Observable.Start(async () => await LoadFarmLists(accountId), RxApp.MainThreadScheduler);
        }

        protected override async Task Load(int accountId)
        {
            await LoadFarmLists(accountId);
            await LoadSettings(accountId);
        }

        private async Task AddLoadTask()
        {
            await Task.Run(() =>
            {
                var task = _taskManager.Get<UpdateFarmListTask>(AccountId);
                if (task is null)
                {
                    _taskManager.Add<UpdateFarmListTask>(AccountId);
                }
                else
                {
                    task.ExecuteAt = DateTime.Now;
                    _taskManager.ReOrder(AccountId);
                }
            });
            await _messageBoxViewModel.Show("Information", "Added update farm list task");
        }

        private async Task StartTask()
        {
            if (!FarmListSettingInput.UseStartAllButton)
            {
                var count = await _farmListRepository.CountActiveFarmLists(AccountId);
                if (count == 0)
                {
                    await _messageBoxViewModel.Show("Information", "There is no active farm or use start all button is disable");
                    return;
                }
            }
            await Task.Run(() =>
            {
                var task = _taskManager.Get<StartFarmListTask>(AccountId);
                if (task is null)
                {
                    _taskManager.Add<StartFarmListTask>(AccountId);
                }
            });
            await _messageBoxViewModel.Show("Information", "Added start farm list task");
        }

        private async Task StopTask()
        {
            await Task.Run(() =>
            {
                var task = _taskManager.Get<StartFarmListTask>(AccountId);
                if (task is not null)
                {
                    _taskManager.Remove(AccountId, task);
                }
            });
            await _messageBoxViewModel.Show("Information", "Removed start farm list task");
        }

        private async Task Save(int accountId)
        {
            var settings = FarmListSettingInput.Get();
            await _accountSettingRepository.Set(accountId, settings);
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
            if (SelectedFarmList is null)
            {
                await _messageBoxViewModel.Show("Warning", "No farm list selected");
                return;
            }
            await _farmListRepository.ActiveFarmList(SelectedFarmList.Id);
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
            var settings = await _accountSettingRepository.Get(accountId);
            FarmListSettingInput.Set(settings);
        }

        private async Task LoadFarmLists(int accountId)
        {
            var farmLists = await _farmListRepository.GetList(accountId);
            FarmLists.Clear();
            foreach (var farmList in farmLists)
            {
                FarmLists.Add(new(farmList));
            }

            if (farmLists.Count > 0)
            {
                SelectedFarmList = FarmLists[0];
            }
            else
            {
                SelectedFarmList = null;
            }
        }

        public ObservableCollection<ListBoxItem> FarmLists { get; } = new();
        private ListBoxItem _selectedFarmList;

        public ListBoxItem SelectedFarmList
        {
            get => _selectedFarmList;
            set => this.RaiseAndSetIfChanged(ref _selectedFarmList, value);
        }

        public ReactiveCommand<Unit, Unit> LoadFarmListCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ActiveFarmListCommand { get; }
        public ReactiveCommand<Unit, Unit> StartCommand { get; }
        public ReactiveCommand<Unit, Unit> StopCommand { get; }
    }
}