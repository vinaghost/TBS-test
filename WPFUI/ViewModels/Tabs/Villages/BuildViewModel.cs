using FluentValidation;
using MainCore.Enums;
using MainCore.Models;
using MainCore.Models.Plans;
using MainCore.Repositories;
using MainCore.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using UpgradeBuildingCore.Tasks;
using WPFUI.Models.Input;
using WPFUI.Models.Output;
using WPFUI.Repositories;
using WPFUI.Services;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.UserControls;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class BuildViewModel : VillageTabViewModelBase
    {
        private readonly IBuildingRepository _buildingRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IBuildRepository _buildRepository;
        private readonly IMessageService _messageService;
        private readonly ITaskManager _taskManager;

        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        public BuildViewModel(IBuildingRepository buildingRepository, IJobRepository jobRepository, IBuildRepository buildRepository, IValidator<NormalBuildInput> normalBuildInputValidator, IMessageService messageService, WaitingOverlayViewModel waitingOverlayViewModel, IValidator<ResourceBuildInput> resourceBuildInputValidator, ITaskManager taskManager)
        {
            _buildingRepository = buildingRepository;
            _jobRepository = jobRepository;
            _buildRepository = buildRepository;
            _normalBuildInputValidator = normalBuildInputValidator;
            _resourceBuildInputValidator = resourceBuildInputValidator;
            _messageService = messageService;
            _waitingOverlayViewModel = waitingOverlayViewModel;
            _taskManager = taskManager;

            _buildingRepository.BuildingUpdated += BuildingUpdated;
            _jobRepository.Locked += Locked;
            _jobRepository.AddActionCompleted += AddActionCompleted;
            _jobRepository.DeleteActionCompleted += DeleteActionCompleted;

            NormalBuildCommand = ReactiveCommand.CreateFromTask(NormalBuildTask);
            ResourceBuildCommand = ReactiveCommand.CreateFromTask(ResourceBuildTask);

            var jobObservable = this.WhenAnyValue(vm => vm.IsEnableJob);
            UpCommand = ReactiveCommand.CreateFromTask(UpTask, jobObservable);
            DownCommand = ReactiveCommand.CreateFromTask(DownTask, jobObservable);
            TopCommand = ReactiveCommand.CreateFromTask(TopTask, jobObservable);
            BottomCommand = ReactiveCommand.CreateFromTask(BottomTask, jobObservable);
            DeleteCommand = ReactiveCommand.CreateFromTask(DeleteTask, jobObservable);
            DeleteAllCommand = ReactiveCommand.CreateFromTask(DeleteAllTask, jobObservable);

            this.WhenAnyValue(vm => vm.SelectedBuilding)
                .Subscribe(async x => await LoadNormalBuild());
        }

        private async Task DeleteActionCompleted(int villageId, Job job)
        {
            if (!IsActive) return;
            if (villageId != VillageId) return;
            await Observable.Start(async () =>
            {
                var uiJob = Jobs.FirstOrDefault(x => x.Id == job.Id);
                Jobs.Remove(uiJob);
                SelectedJob = Jobs[0];

                IsEnableNormalBuild = true;
                IsEnableResourceBuild = true;
                IsEnableJob = true;
                await LoadBuildings(VillageId);
            }, RxApp.MainThreadScheduler);
        }

        private async Task AddActionCompleted(int villageId, Job job)
        {
            if (!IsActive) return;
            if (villageId != VillageId) return;
            await Observable.Start(async () =>
            {
                Jobs.Insert(0, new(job));
                CheckBuildings();
                IsEnableJob = true;
                await LoadBuildings(VillageId);
            }, RxApp.MainThreadScheduler);
        }

        private async Task Locked(int villageId)
        {
            if (!IsActive) return;
            if (villageId != VillageId) return;
            await Observable.Start(() =>
            {
                CheckBuildings();
                IsEnableJob = false;
            }, RxApp.MainThreadScheduler);
        }

        private async Task BuildingUpdated(int villageId)
        {
            if (!IsActive) return;
            if (villageId != VillageId) return;
            await Observable.Start(async () => await LoadBuildings(villageId), RxApp.MainThreadScheduler);
        }

        protected override async Task Load(int villageId)
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

        private async Task LoadBuildings(int villageId)
        {
            var buildings = await _buildRepository.GetBuildingItems(villageId);
            Buildings.Clear();
            foreach (var building in buildings)
            {
                Buildings.Add(new(building));
            }

            if (buildings.Count > 0)
            {
                SelectedBuilding = Buildings[0];
            }
            else
            {
                SelectedBuilding = null;
            }
        }

        private async Task LoadJobs(int villageId)
        {
            IsEnableJob = true;
            var jobs = await _jobRepository.GetList(villageId);
            Jobs.Clear();
            foreach (var job in jobs)
            {
                Jobs.Add(new(job));
            }

            if (jobs.Count > 0)
            {
                SelectedJob = Jobs[0];
            }
            else
            {
                SelectedJob = null;
            }
        }

        private async Task LoadNormalBuild()
        {
            if (SelectedBuilding is null)
            {
                IsEnableNormalBuild = false;
                NormalBuildInput.Clear();
                return;
            }
            IsEnableNormalBuild = true;

            var building = await _buildingRepository.Get(SelectedBuilding.Id);
            if (building.Type == BuildingEnums.Site)
            {
                var buildings = _buildRepository.GetAvailableBuildings();
                NormalBuildInput.Set(buildings);
            }
            else
            {
                NormalBuildInput.Set(new() { building.Type }, building.Level + 1);
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
                _messageService.Show("Error", result.ToString());
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
                _messageService.Show("Error", result.ToString());
            }
            else
            {
                await ResourceBuild(VillageId);
                AddTask();
            }
        }

        private void AddTask()
        {
            var task = _taskManager.Get<NormalUpgradeBuildingTask>(AccountId, VillageId);
            if (task is null)
            {
                _taskManager.Add<NormalUpgradeBuildingTask>(AccountId, VillageId);
            }
            else
            {
                task.ExecuteAt = DateTime.Now;
            }
        }

        private async Task UpTask()
        {
            if (SelectedJob is null) return;

            var oldIndex = SelectedJobIndex;

            if (oldIndex == 0) return;
            var newIndex = oldIndex - 1;

            var oldJob = Jobs[oldIndex];
            var newJob = Jobs[newIndex];

            Jobs.Move(oldIndex, newIndex);
            SelectedJobIndex = newIndex;

            await _jobRepository.Move(oldJob.Id, newJob.Id);
        }

        private async Task DownTask()
        {
            if (SelectedJob is null) return;

            var oldIndex = SelectedJobIndex;

            if (oldIndex == Jobs.Count - 1) return;
            var newIndex = oldIndex + 1;

            var oldJob = Jobs[oldIndex];
            var newJob = Jobs[newIndex];

            Jobs.Move(oldIndex, newIndex);
            SelectedJobIndex = newIndex;

            await _jobRepository.Move(oldJob.Id, newJob.Id);
        }

        private async Task TopTask()
        {
            if (SelectedJob is null) return;

            var oldIndex = SelectedJobIndex;

            if (oldIndex == 0) return;
            var newIndex = 0;

            var oldJob = Jobs[oldIndex];
            var newJob = Jobs[newIndex];

            Jobs.Move(oldIndex, newIndex);
            SelectedJobIndex = newIndex;

            await _jobRepository.Move(oldJob.Id, newJob.Id);
        }

        private async Task BottomTask()
        {
            if (SelectedJob is null) return;

            var oldIndex = SelectedJobIndex;

            if (oldIndex == Jobs.Count - 1) return;
            var newIndex = Jobs.Count - 1;

            var oldJob = Jobs[oldIndex];
            var newJob = Jobs[newIndex];

            Jobs.Move(oldIndex, newIndex);
            SelectedJobIndex = newIndex;

            await _jobRepository.Move(oldJob.Id, newJob.Id);
        }

        private async Task DeleteTask()
        {
            if (SelectedJob is null) return;
            var jobId = SelectedJob.Id;
            var oldIndex = SelectedJobIndex;
            Jobs.RemoveAt(oldIndex);
            await _jobRepository.Delete(jobId);
            if (Jobs.Count > 0)
            {
                if (oldIndex == Jobs.Count)
                {
                    oldIndex = Jobs.Count - 1;
                }

                SelectedJobIndex = oldIndex;
            }
            await LoadBuildings(VillageId);
        }

        private async Task DeleteAllTask()
        {
            Jobs.Clear();
            await _jobRepository.Clear(VillageId);
            await LoadBuildings(VillageId);
        }

        private async Task NormalBuild(int villageId)
        {
            var building = await _buildingRepository.Get(SelectedBuilding.Id);
            var (type, level) = NormalBuildInput.Get();
            var plan = new NormalBuildPlan()
            {
                Location = building.Location,
                Type = type,
                Level = level,
            };
            var result = await _buildRepository.CheckRequirements(VillageId, plan);
            if (result.IsFailed)
            {
                _messageService.Show("Error", result.Errors.First().Message);
                return;
            }
            await _buildRepository.Validate(VillageId, plan);
            var job = await _jobRepository.Add(villageId, plan);
            Jobs.Add(new(job));
            await LoadBuildings(VillageId);
        }

        private async Task ResourceBuild(int villageId)
        {
            var (type, level) = ResourceBuildInput.Get();
            var plan = new ResourceBuildPlan()
            {
                Plan = type,
                Level = level,
            };
            var job = await _jobRepository.Add(villageId, plan);

            Jobs.Add(new(job));
            await LoadBuildings(VillageId);
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

        public ObservableCollection<ListBoxItem> Buildings { get; } = new();
        private ListBoxItem _selectedBuilding;

        public ListBoxItem SelectedBuilding
        {
            get => _selectedBuilding;
            set => this.RaiseAndSetIfChanged(ref _selectedBuilding, value);
        }

        public ObservableCollection<ListBoxItem> Jobs { get; } = new();
        private ListBoxItem _selectedJob;

        public ListBoxItem SelectedJob
        {
            get => _selectedJob;
            set => this.RaiseAndSetIfChanged(ref _selectedJob, value);
        }

        private int _selectedJobIndex;

        public int SelectedJobIndex
        {
            get => _selectedJobIndex;
            set => this.RaiseAndSetIfChanged(ref _selectedJobIndex, value);
        }

        private bool _isEnableJob;

        public bool IsEnableJob
        {
            get => _isEnableJob;
            set => this.RaiseAndSetIfChanged(ref _isEnableJob, value);
        }
    }
}