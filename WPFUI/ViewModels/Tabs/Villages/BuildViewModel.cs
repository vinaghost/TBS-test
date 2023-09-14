using FluentValidation;
using MainCore.Enums;
using MainCore.Models.Plans;
using MainCore.Repositories;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
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

        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        public BuildViewModel(IBuildingRepository buildingRepository, IJobRepository jobRepository, IBuildRepository buildRepository, IValidator<NormalBuildInput> normalBuildInputValidator, IMessageService messageService, WaitingOverlayViewModel waitingOverlayViewModel)
        {
            _buildingRepository = buildingRepository;
            _jobRepository = jobRepository;
            _buildRepository = buildRepository;
            _normalBuildInputValidator = normalBuildInputValidator;
            _messageService = messageService;
            _waitingOverlayViewModel = waitingOverlayViewModel;

            _buildingRepository.BuildingUpdated += BuildingUpdated;

            NormalBuildCommand = ReactiveCommand.CreateFromTask(NormalBuildTask);

            this.WhenAnyValue(vm => vm.SelectedBuilding)
                .Subscribe(async x => await LoadNormalBuild());
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
        }

        private async Task LoadBuildings(int villageId)
        {
            var buildings = await _buildingRepository.GetList(villageId);
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

        public async Task LoadNormalBuild()
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
                NormalBuildInput.Set(buildings, 20);
            }
            else
            {
                NormalBuildInput.Set(new() { building.Type }, 20);
            }
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
            }
        }

        private async Task NormalBuild(int villageId)
        {
            var building = await _buildingRepository.Get(SelectedBuilding.Id);
            var (type, level) = NormalBuildInput.Get();
            var plan = new NormalBuildPlan()
            {
                Location = building.Location,
                Building = type,
                Level = level,
            };
            var job = await _jobRepository.Add(villageId, plan);

            Jobs.Add(new(job));
        }

        public ReactiveCommand<Unit, Unit> NormalBuildCommand { get; }
        public NormalBuildInput NormalBuildInput { get; } = new();
        private readonly IValidator<NormalBuildInput> _normalBuildInputValidator;

        private bool _isEnableNormalBuild;

        public bool IsEnableNormalBuild
        {
            get => _isEnableNormalBuild;
            set => this.RaiseAndSetIfChanged(ref _isEnableNormalBuild, value);
        }

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
    }
}