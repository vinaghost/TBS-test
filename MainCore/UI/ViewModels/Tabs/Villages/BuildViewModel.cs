using FluentResults;
using FluentValidation;
using Humanizer;
using MainCore.Common.Enums;
using MainCore.Common.Extensions;
using MainCore.Common.Models;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Notification.Message;
using MainCore.Repositories;
using MainCore.Services;
using MainCore.Tasks;
using MainCore.UI.Models.Input;
using MainCore.UI.Models.Output;
using MainCore.UI.ViewModels.Abstract;
using MainCore.UI.ViewModels.UserControls;
using MediatR;
using ReactiveUI;
using System.Reactive.Linq;
using Unit = System.Reactive.Unit;

namespace MainCore.UI.ViewModels.Tabs.Villages
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class BuildViewModel : VillageTabViewModelBase, IBuildViewModel
    {
        private readonly ITaskManager _taskManager;
        private readonly IMediator _mediator;
        private readonly IDialogService _dialogService;
        private readonly IUnitOfRepository _unitOfRepository;

        private readonly List<BuildingEnums> _availableBuildings = new();

        public BuildViewModel(IValidator<NormalBuildInput> normalBuildInputValidator, IValidator<ResourceBuildInput> resourceBuildInputValidator, ITaskManager taskManager, IDialogService dialogService, IMediator mediator, IUnitOfRepository unitOfRepository)
        {
            _normalBuildInputValidator = normalBuildInputValidator;
            _resourceBuildInputValidator = resourceBuildInputValidator;

            _taskManager = taskManager;
            _dialogService = dialogService;
            _unitOfRepository = unitOfRepository;
            _mediator = mediator;

            LoadNormalBuildCommand = ReactiveCommand.CreateFromTask<ListBoxItem>(LoadNormalBuild);
            NormalBuildCommand = ReactiveCommand.CreateFromTask(NormalBuildTask);
            ResourceBuildCommand = ReactiveCommand.CreateFromTask(ResourceBuildTask);

            UpCommand = ReactiveCommand.CreateFromTask(UpTask);
            DownCommand = ReactiveCommand.CreateFromTask(DownTask);
            TopCommand = ReactiveCommand.CreateFromTask(TopTask);
            BottomCommand = ReactiveCommand.CreateFromTask(BottomTask);
            DeleteCommand = ReactiveCommand.CreateFromTask(DeleteTask);
            DeleteAllCommand = ReactiveCommand.CreateFromTask(DeleteAllTask);

            this.WhenAnyValue(vm => vm.Buildings.SelectedItem)
                .InvokeCommand(LoadNormalBuildCommand);

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
            await LoadJobs(villageId);
            await LoadBuildings(villageId);
        }

        private async Task LoadBuildings(VillageId villageId)
        {
            var buildings = await Task.Run(() => _unitOfRepository.BuildingRepository.GetItems(villageId));
            await Observable.Start(() =>
            {
                Buildings.Load(buildings);
            }, RxApp.MainThreadScheduler);
        }

        private async Task LoadJobs(VillageId villageId)
        {
            var jobs = await Task.Run(() => _unitOfRepository.JobRepository.GetItems(villageId));
            await Observable.Start(() =>
            {
                Jobs.Load(jobs);
            }, RxApp.MainThreadScheduler);
        }

        private async Task LoadNormalBuild(ListBoxItem item)
        {
            Action func;
            if (item is null)
            {
                func = () => NormalBuildInput.Clear();
            }
            else
            {
                var (type, level) = await Task.Run(() => _unitOfRepository.BuildingRepository.GetBuildingInfo(new BuildingId(Buildings.SelectedItemId)));

                if (type == BuildingEnums.Site)
                {
                    func = () => NormalBuildInput.Set(_availableBuildings);
                }
                else
                {
                    func = () => NormalBuildInput.Set(new List<BuildingEnums>() { type }, level + 1);
                }
            }

            await Observable.Start(func, RxApp.MainThreadScheduler);
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

            await Task.Run(() => _unitOfRepository.JobRepository.Move(new JobId(oldJob.Id), new JobId(newJob.Id)));
            await _mediator.Publish(new JobUpdated(AccountId, VillageId));
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

            await Task.Run(() => _unitOfRepository.JobRepository.Move(new JobId(oldJob.Id), new JobId(newJob.Id)));
            await _mediator.Publish(new JobUpdated(AccountId, VillageId));
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

            await Task.Run(() => _unitOfRepository.JobRepository.Move(new JobId(oldJob.Id), new JobId(newJob.Id)));
            await _mediator.Publish(new JobUpdated(AccountId, VillageId));
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

            await Task.Run(() => _unitOfRepository.JobRepository.Move(new JobId(oldJob.Id), new JobId(newJob.Id)));
            await _mediator.Publish(new JobUpdated(AccountId, VillageId));
            Jobs.SelectedIndex = newIndex;
        }

        private async Task DeleteTask()
        {
            if (!Jobs.IsSelected) return;
            var oldIndex = Jobs.SelectedIndex;
            var jobId = Jobs.SelectedItemId;

            await Task.Run(() => _unitOfRepository.JobRepository.Delete(new JobId(jobId)));
            await _mediator.Publish(new JobUpdated(AccountId, VillageId));
        }

        private async Task DeleteAllTask()
        {
            await Task.Run(() => _unitOfRepository.JobRepository.Delete(VillageId));
            await _mediator.Publish(new JobUpdated(AccountId, VillageId));
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

            var buildings = await Task.Run(() => _unitOfRepository.BuildingRepository.GetLevelBuildings(villageId));
            var result = CheckRequirements(buildings, plan);
            if (result.IsFailed)
            {
                _dialogService.ShowMessageBox("Error", result.Errors.First().Message);
                return;
            }
            Validate(buildings, plan);

            await Task.Run(() => _unitOfRepository.JobRepository.Add(villageId, plan));
            await _mediator.Publish(new JobUpdated(AccountId, VillageId));
        }

        private async Task ResourceBuild(VillageId villageId)
        {
            var (type, level) = ResourceBuildInput.Get();
            var plan = new ResourceBuildPlan()
            {
                Plan = type,
                Level = level,
            };
            await Task.Run(() => _unitOfRepository.JobRepository.Add(villageId, plan));
            await _mediator.Publish(new JobUpdated(AccountId, VillageId));
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

        public ReactiveCommand<ListBoxItem, Unit> LoadNormalBuildCommand { get; }

        public ReactiveCommand<Unit, Unit> NormalBuildCommand { get; }
        public ReactiveCommand<Unit, Unit> ResourceBuildCommand { get; }

        public ReactiveCommand<Unit, Unit> UpCommand { get; }
        public ReactiveCommand<Unit, Unit> DownCommand { get; }
        public ReactiveCommand<Unit, Unit> TopCommand { get; }
        public ReactiveCommand<Unit, Unit> BottomCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAllCommand { get; }

        public NormalBuildInput NormalBuildInput { get; } = new();
        private readonly IValidator<NormalBuildInput> _normalBuildInputValidator;

        public ResourceBuildInput ResourceBuildInput { get; } = new();
        private readonly IValidator<ResourceBuildInput> _resourceBuildInputValidator;

        public ListBoxItemViewModel Buildings { get; } = new();
        public ListBoxItemViewModel Jobs { get; } = new();
    }
}