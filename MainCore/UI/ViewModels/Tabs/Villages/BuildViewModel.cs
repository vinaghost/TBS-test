using FluentValidation;
using MainCore.Common.Enums;
using MainCore.Common.Models;
using MainCore.Common.Repositories;
using MainCore.Entities;
using MainCore.Features.UpgradeBuilding.Tasks;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.UI.Models.Input;
using MainCore.UI.Models.Output;
using MainCore.UI.ViewModels.Abstract;
using MainCore.UI.ViewModels.UserControls;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace MainCore.UI.ViewModels.Tabs.Villages
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class BuildViewModel : VillageTabViewModelBase
    {
        private readonly IBuildingRepository _buildingRepository;
        private readonly IJobRepository _jobRepository;

        private readonly ITaskManager _taskManager;

        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;
        private readonly IDialogService _dialogService;

        public BuildViewModel(IJobRepository jobRepository, IBuildingRepository buildingRepository, IValidator<NormalBuildInput> normalBuildInputValidator, WaitingOverlayViewModel waitingOverlayViewModel, IValidator<ResourceBuildInput> resourceBuildInputValidator, ITaskManager taskManager, IDialogService dialogService)
        {
            _buildingRepository = buildingRepository;
            _jobRepository = jobRepository;
            _normalBuildInputValidator = normalBuildInputValidator;
            _resourceBuildInputValidator = resourceBuildInputValidator;

            _waitingOverlayViewModel = waitingOverlayViewModel;
            _taskManager = taskManager;
            _dialogService = dialogService;

            NormalBuildCommand = ReactiveCommand.CreateFromTask(NormalBuildTask);
            ResourceBuildCommand = ReactiveCommand.CreateFromTask(ResourceBuildTask);

            var jobObservable = this.WhenAnyValue(vm => vm.IsEnableJob);
            UpCommand = ReactiveCommand.CreateFromTask(UpTask, jobObservable);
            DownCommand = ReactiveCommand.CreateFromTask(DownTask, jobObservable);
            TopCommand = ReactiveCommand.CreateFromTask(TopTask, jobObservable);
            BottomCommand = ReactiveCommand.CreateFromTask(BottomTask, jobObservable);
            DeleteCommand = ReactiveCommand.CreateFromTask(DeleteTask, jobObservable);
            DeleteAllCommand = ReactiveCommand.CreateFromTask(DeleteAllTask, jobObservable);

            this.WhenAnyValue(vm => vm.Buildings.SelectedItem)
                .Subscribe(async x => await LoadNormalBuild());
        }

        public async Task BuildingUpdate(VillageId villageId)
        {
            if (!IsActive) return;
            if (villageId != VillageId) return;
            await Observable.Start(
                async () => await LoadBuildings(villageId),
                RxApp.MainThreadScheduler);
        }

        public async Task JobUpdate(VillageId villageId)
        {
            if (!IsActive) return;
            if (villageId != VillageId) return;
            await Observable.Start(
                async () => await LoadJobs(villageId),
                RxApp.MainThreadScheduler);
        }

        protected override async Task Load(VillageId villageId)
        {
            await LoadBuildings(villageId);
            await LoadJobs(villageId);
            LoadResourceBuild();
            CheckBuildings();
        }

        private void CheckBuildings()
        {
            if (Buildings.Count == 40)
            {
                IsEnableNormalBuild = true;
                IsEnableResourceBuild = true;
            }
            else
            {
                IsEnableNormalBuild = false;
                IsEnableResourceBuild = false;
            }
        }

        private async Task LoadBuildings(VillageId villageId)
        {
            var buildings = await Task.Run(() => _buildingRepository.GetBuildingItems(villageId));
            Buildings.Load(buildings.Select(x => new ListBoxItem(x)));
        }

        private async Task LoadJobs(VillageId villageId)
        {
            IsEnableJob = true;
            var jobs = await _jobRepository.GetAll(villageId);
            Jobs.Load(jobs.Select(x => new ListBoxItem(x)));
        }

        private async Task LoadNormalBuild()
        {
            if (!Buildings.IsSelected)
            {
                IsEnableNormalBuild = false;
                NormalBuildInput.Clear();
                return;
            }
            IsEnableNormalBuild = true;

            var building = await Task.Run(() => _buildingRepository.GetBuilding(Buildings.SelectedItemId));
            if (building.Type == BuildingEnums.Site)
            {
                var buildings = _buildingRepository.AvailableBuildings;
                NormalBuildInput.Set(buildings);
            }
            else
            {
                NormalBuildInput.Set(new List<BuildingEnums>() { building.Type }, building.Level + 1);
            }
        }

        private void LoadResourceBuild()
        {
            if (Buildings.Count == 0)
            {
                IsEnableResourceBuild = false;
                return;
            }
            IsEnableResourceBuild = true;
        }

        private async Task NormalBuildTask()
        {
            var result = _normalBuildInputValidator.Validate(NormalBuildInput);
            if (!result.IsValid)
            {
                _dialogService.ShowMessageBox("Error", result.ToString());
            }
            else
            {
                await NormalBuild(VillageId);
                AddTask();
            }
        }

        private async Task ResourceBuildTask()
        {
            var result = _resourceBuildInputValidator.Validate(ResourceBuildInput);
            if (!result.IsValid)
            {
                _dialogService.ShowMessageBox("Error", result.ToString());
            }
            else
            {
                await ResourceBuild(VillageId);
                AddTask();
            }
        }

        private void AddTask()
        {
            _taskManager.AddOrUpdate<UpgradeBuildingTask>(AccountId, VillageId);
        }

        private async Task UpTask()
        {
            if (!Jobs.IsSelected) return;

            var oldIndex = Jobs.SelectedIndex;

            if (oldIndex == 0) return;
            var newIndex = oldIndex - 1;

            var oldJob = Jobs[oldIndex];
            var newJob = Jobs[newIndex];

            Jobs.Move(oldIndex, newIndex);
            Jobs.SelectedIndex = newIndex;

            await _jobRepository.Move(new JobId(oldJob.Id), new JobId(newJob.Id));
        }

        private async Task DownTask()
        {
            if (!Jobs.IsSelected) return;

            var oldIndex = Jobs.SelectedIndex;

            if (oldIndex == Jobs.Count - 1) return;
            var newIndex = oldIndex + 1;

            var oldJob = Jobs[oldIndex];
            var newJob = Jobs[newIndex];

            Jobs.Move(oldIndex, newIndex);
            Jobs.SelectedIndex = newIndex;

            await _jobRepository.Move(new JobId(oldJob.Id), new JobId(newJob.Id));
        }

        private async Task TopTask()
        {
            if (!Jobs.IsSelected) return;

            var oldIndex = Jobs.SelectedIndex;

            if (oldIndex == 0) return;
            var newIndex = 0;

            var oldJob = Jobs[oldIndex];
            var newJob = Jobs[newIndex];

            Jobs.Move(oldIndex, newIndex);
            Jobs.SelectedIndex = newIndex;

            await _jobRepository.Move(new JobId(oldJob.Id), new JobId(newJob.Id));
        }

        private async Task BottomTask()
        {
            if (!Jobs.IsSelected) return;

            var oldIndex = Jobs.SelectedIndex;

            if (oldIndex == Jobs.Count - 1) return;
            var newIndex = Jobs.Count - 1;

            var oldJob = Jobs[oldIndex];
            var newJob = Jobs[newIndex];

            Jobs.Move(oldIndex, newIndex);
            Jobs.SelectedIndex = newIndex;

            await _jobRepository.Move(new JobId(oldJob.Id), new JobId(newJob.Id));
        }

        private async Task DeleteTask()
        {
            if (!Jobs.IsSelected) return;
            var jobId = Jobs.SelectedItemId;
            Jobs.Delete();
            await _jobRepository.DeleteById(new JobId(jobId));
        }

        private async Task DeleteAllTask()
        {
            await _jobRepository.Clear(VillageId);
        }

        private async Task NormalBuild(VillageId villageId)
        {
            var building = _buildingRepository.GetBuilding(Buildings.SelectedItemId);
            var (type, level) = NormalBuildInput.Get();
            var plan = new NormalBuildPlan()
            {
                Location = building.Location,
                Type = type,
                Level = level,
            };
            var result = _buildingRepository.CheckRequirements(VillageId, plan);
            if (result.IsFailed)
            {
                _dialogService.ShowMessageBox("Error", result.Errors.First().Message);
                return;
            }
            _buildingRepository.Validate(VillageId, plan);
            await _jobRepository.Add(villageId, plan);
        }

        private async Task ResourceBuild(VillageId villageId)
        {
            var (type, level) = ResourceBuildInput.Get();
            var plan = new ResourceBuildPlan()
            {
                Plan = type,
                Level = level,
            };
            await _jobRepository.Add(villageId, plan);
        }

        public ReactiveCommand<Unit, Unit> NormalBuildCommand { get; }
        public ReactiveCommand<Unit, Unit> ResourceBuildCommand { get; }

        public ReactiveCommand<Unit, Unit> UpCommand { get; }
        public ReactiveCommand<Unit, Unit> DownCommand { get; }
        public ReactiveCommand<Unit, Unit> TopCommand { get; }
        public ReactiveCommand<Unit, Unit> BottomCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAllCommand { get; }

        #region Normal build input

        public NormalBuildInput NormalBuildInput { get; } = new();
        private readonly IValidator<NormalBuildInput> _normalBuildInputValidator;

        private bool _isEnableNormalBuild;

        public bool IsEnableNormalBuild
        {
            get => _isEnableNormalBuild;
            set => this.RaiseAndSetIfChanged(ref _isEnableNormalBuild, value);
        }

        #endregion Normal build input

        #region Resource build input

        public ResourceBuildInput ResourceBuildInput { get; } = new();
        private readonly IValidator<ResourceBuildInput> _resourceBuildInputValidator;
        private bool _isEnableResourceBuild;

        public bool IsEnableResourceBuild
        {
            get => _isEnableResourceBuild;
            set => this.RaiseAndSetIfChanged(ref _isEnableResourceBuild, value);
        }

        #endregion Resource build input

        public ListBoxItemViewModel Buildings { get; } = new();
        public ListBoxItemViewModel Jobs { get; } = new();

        private bool _isEnableJob;

        public bool IsEnableJob
        {
            get => _isEnableJob;
            set => this.RaiseAndSetIfChanged(ref _isEnableJob, value);
        }
    }
}