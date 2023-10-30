using FluentResults;
using FluentValidation;
using Humanizer;
using MainCore.Common.Enums;
using MainCore.Common.Extensions;
using MainCore.Common.Models;
using MainCore.Common.Repositories;
using MainCore.CQRS.Commands;
using MainCore.CQRS.Queries;
using MainCore.Entities;
using MainCore.Features.UpgradeBuilding.Tasks;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.UI.Models.Input;
using MainCore.UI.ViewModels.Abstract;
using MainCore.UI.ViewModels.UserControls;
using MediatR;
using ReactiveUI;
using System.Reactive.Linq;
using Unit = System.Reactive.Unit;

namespace MainCore.UI.ViewModels.Tabs.Villages
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class BuildViewModel : VillageTabViewModelBase
    {
        private readonly ITaskManager _taskManager;
        private readonly IMediator _mediator;
        private readonly IDialogService _dialogService;

        private readonly List<BuildingEnums> _availableBuildings = new();

        public BuildViewModel(IValidator<NormalBuildInput> normalBuildInputValidator, IValidator<ResourceBuildInput> resourceBuildInputValidator, ITaskManager taskManager, IDialogService dialogService, IMediator mediator)
        {
            _normalBuildInputValidator = normalBuildInputValidator;
            _resourceBuildInputValidator = resourceBuildInputValidator;

            _taskManager = taskManager;
            _dialogService = dialogService;
            _mediator = mediator;

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

            for (var i = BuildingEnums.Sawmill; i <= BuildingEnums.Hospital; i++)
            {
                _availableBuildings.Add(i);
            }
        }

        public async Task BuildingListRefresh(VillageId villageId)
        {
            if (!IsActive) return;
            if (villageId != VillageId) return;
            await LoadBuildings(villageId);
        }

        public async Task JobListRefresh(VillageId villageId)
        {
            if (!IsActive) return;
            if (villageId != VillageId) return;
            await LoadJobs(villageId);
            await LoadBuildings(villageId);
        }

        protected override async Task Load(VillageId villageId)
        {
            await LoadBuildings(villageId);
            await LoadJobs(villageId);
            LoadResourceBuild();
            ValidateVillage();
        }

        private void ValidateVillage()
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
            var buildings = await _mediator.Send(new GetBuildingListBoxItemsQuery(villageId));
            await Observable.Start(() =>
            {
                Buildings.Load(buildings);
            }, RxApp.MainThreadScheduler);
        }

        private async Task LoadJobs(VillageId villageId)
        {
            IsEnableJob = true;
            var jobs = await _mediator.Send(new GetJobListBoxItemsQuery(villageId));
            await Observable.Start(() =>
            {
                Jobs.Load(jobs);
            }, RxApp.MainThreadScheduler);
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

            var building = await _mediator.Send(new GetBuildingByIdQuery(new BuildingId(Buildings.SelectedItemId)));
            if (building.Type == BuildingEnums.Site)
            {
                NormalBuildInput.Set(_availableBuildings);
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
                return;
            }
            await NormalBuild(VillageId);
            AddTask();
        }

        private async Task ResourceBuildTask()
        {
            var result = _resourceBuildInputValidator.Validate(ResourceBuildInput);
            if (!result.IsValid)
            {
                _dialogService.ShowMessageBox("Error", result.ToString());
                return;
            }
            await ResourceBuild(VillageId);
            AddTask();
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

            await _mediator.Send(new MoveJobCommand(new JobId(oldJob.Id), new JobId(newJob.Id)));
            Jobs.SelectedIndex = newIndex;
        }

        private async Task DownTask()
        {
            if (!Jobs.IsSelected) return;

            var oldIndex = Jobs.SelectedIndex;

            if (oldIndex == Jobs.Count - 1) return;
            var newIndex = oldIndex + 1;

            var oldJob = Jobs[oldIndex];
            var newJob = Jobs[newIndex];

            await _mediator.Send(new MoveJobCommand(new JobId(oldJob.Id), new JobId(newJob.Id)));
            Jobs.SelectedIndex = newIndex;
        }

        private async Task TopTask()
        {
            if (!Jobs.IsSelected) return;

            var oldIndex = Jobs.SelectedIndex;

            if (oldIndex == 0) return;
            var newIndex = 0;

            var oldJob = Jobs[oldIndex];
            var newJob = Jobs[newIndex];

            await _mediator.Send(new MoveJobCommand(new JobId(oldJob.Id), new JobId(newJob.Id)));
            Jobs.SelectedIndex = newIndex;
        }

        private async Task BottomTask()
        {
            if (!Jobs.IsSelected) return;

            var oldIndex = Jobs.SelectedIndex;

            if (oldIndex == Jobs.Count - 1) return;
            var newIndex = Jobs.Count - 1;

            var oldJob = Jobs[oldIndex];
            var newJob = Jobs[newIndex];

            await _mediator.Send(new MoveJobCommand(new JobId(oldJob.Id), new JobId(newJob.Id)));
            Jobs.SelectedIndex = newIndex;
        }

        private async Task DeleteTask()
        {
            if (!Jobs.IsSelected) return;
            var oldIndex = Jobs.SelectedIndex;
            var jobId = Jobs.SelectedItemId;
            await _mediator.Send(new DeleteJobByIdCommand(new JobId(jobId)));
            if (!Jobs.IsSelected) return;
            if (oldIndex < Jobs.Count)
            {
                Jobs.SelectedIndex = oldIndex;
            }
            else
            {
                Jobs.SelectedIndex = Jobs.Count - 1;
            }
        }

        private async Task DeleteAllTask()
        {
            await _mediator.Send(new DeleteAllJobCommand(VillageId));
        }

        private async Task NormalBuild(VillageId villageId)
        {
            var location = Buildings.SelectedIndex + 1;
            var (type, level) = NormalBuildInput.Get();
            var plan = new NormalBuildPlan()
            {
                Location = location,
                Type = type,
                Level = level,
            };

            var buildings = await _mediator.Send(new GetBuildingsWithPlannedLevelQuery(villageId));
            var result = CheckRequirements(buildings, plan);
            if (result.IsFailed)
            {
                _dialogService.ShowMessageBox("Error", result.Errors.First().Message);
                return;
            }
            Validate(buildings, plan);

            await _mediator.Send(new AddJobCommand<NormalBuildPlan>(VillageId, plan));
        }

        private async Task ResourceBuild(VillageId villageId)
        {
            var (type, level) = ResourceBuildInput.Get();
            var plan = new ResourceBuildPlan()
            {
                Plan = type,
                Level = level,
            };
            await _mediator.Send(new AddJobCommand<ResourceBuildPlan>(villageId, plan));
        }

        private static Result CheckRequirements(List<BuildingItem> buildings, NormalBuildPlan plan)
        {
            var prerequisiteBuildings = plan.Type.GetPrerequisiteBuildings();
            if (prerequisiteBuildings.Count == 0) return Result.Ok();
            foreach (var prerequisiteBuilding in prerequisiteBuildings)
            {
                var building = buildings.FirstOrDefault(x => x.Type == prerequisiteBuilding.Type);
                if (building is null) return Result.Fail($"Required {prerequisiteBuilding.Type.Humanize()} lvl {prerequisiteBuilding.Level}");
                if (building.Level < prerequisiteBuilding.Level) return Result.Fail($"Required {prerequisiteBuilding.Type.Humanize()} lvl {prerequisiteBuilding.Level}");
            }
            return Result.Ok();
        }

        private static void Validate(List<BuildingItem> buildings, NormalBuildPlan plan)
        {
            if (plan.Type.IsWall())
            {
                plan.Location = 40;
                return;
            }
            if (plan.Type.IsMultipleBuilding())
            {
                var sameTypeBuildings = buildings.Where(x => x.Type == plan.Type);
                if (!sameTypeBuildings.Any()) return;
                if (sameTypeBuildings.Where(x => x.Location == plan.Location).Any()) return;
                var largestLevelBuilding = sameTypeBuildings.MaxBy(x => x.Level);
                if (largestLevelBuilding.Level == plan.Type.GetMaxLevel()) return;
                plan.Location = largestLevelBuilding.Location;
                return;
            }

            if (plan.Type.IsResourceField())
            {
                var field = buildings.FirstOrDefault(x => x.Location == plan.Location);
                if (plan.Type == field.Type) return;
                plan.Type = field.Type;
            }

            {
                var building = buildings.FirstOrDefault(x => x.Type == plan.Type);
                if (building is null) return;
                if (plan.Location == building.Location) return;
                plan.Location = building.Location;
            }
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