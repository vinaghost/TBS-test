using MainCore.Common.Repositories;
using MainCore.Features.Update.Tasks;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.UI.Enums;
using MainCore.UI.Models.Output;
using MainCore.UI.Stores;
using MainCore.UI.ViewModels.Abstract;
using MainCore.UI.ViewModels.UserControls;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace MainCore.UI.ViewModels.Tabs
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class VillageViewModel : AccountTabViewModelBase
    {
        private readonly IVillageRepository _villageRepository;
        private readonly VillageTabStore _villageTabStore;

        private readonly ITaskManager _taskManager;
        private readonly MessageBoxViewModel _messageBoxViewModel;
        public ListBoxItemViewModel Villages { get; } = new();

        public VillageViewModel(IVillageRepository villageRepository, VillageTabStore villageTabStore, ITaskManager taskManager, MessageBoxViewModel messageBoxViewModel)
        {
            _villageRepository = villageRepository;
            _villageTabStore = villageTabStore;

            _messageBoxViewModel = messageBoxViewModel;
            _taskManager = taskManager;

            LoadCurrentCommand = ReactiveCommand.CreateFromTask(LoadCurrentTask);
            LoadUnloadCommand = ReactiveCommand.CreateFromTask(LoadUnloadTask);
            LoadAllCommand = ReactiveCommand.CreateFromTask(LoadAllTask);

            var villageObservable = this.WhenAnyValue(x => x.Villages.SelectedItem);
            villageObservable.BindTo(_selectedItemStore, vm => vm.Village);
            villageObservable.Subscribe(x =>
            {
                var tabType = VillageTabType.Normal;
                if (x is null) tabType = VillageTabType.NoVillage;
                _villageTabStore.SetTabType(tabType);
            });
        }

        private async Task LoadCurrentTask()
        {
            if (!Villages.IsSelected)
            {
                await _messageBoxViewModel.Show("Warning", "No village selected");
                return;
            }

            _taskManager.AddOrUpdate<UpdateVillageTask>(AccountId, Villages.SelectedItemId);
            await _messageBoxViewModel.Show("Information", $"Added update task");
            return;
        }

        private async Task LoadUnloadTask()
        {
            if (!Villages.IsSelected)
            {
                await _messageBoxViewModel.Show("Warning", "No village selected");
                return;
            }

            var villages = await _villageRepository.GetUnloadVillageId(AccountId);
            foreach (var village in villages)
            {
                _taskManager.AddOrUpdate<UpdateVillageTask>(AccountId, village);
            }
            await _messageBoxViewModel.Show("Information", $"Added update task");
            return;
        }

        private async Task LoadAllTask()
        {
            if (!Villages.IsSelected)
            {
                await _messageBoxViewModel.Show("Warning", "No village selected");
                return;
            }

            var villages = await Task.Run(() => _villageRepository.GetAll(AccountId));
            foreach (var village in villages)
            {
                _taskManager.AddOrUpdate<UpdateVillageTask>(AccountId, village.Id);
            }
            await _messageBoxViewModel.Show("Information", $"Added update task");
            return;
        }

        public async Task VillageUpdate(int accountId)
        {
            if (!IsActive) return;
            if (AccountId != accountId) return;
            await Observable.Start(
                async () => await LoadVillageList(accountId),
                RxApp.MainThreadScheduler);
        }

        protected override async Task Load(int accountId)
        {
            await LoadVillageList(accountId);
        }

        private async Task LoadVillageList(int accountId)
        {
            var villages = await Task.Run(() => _villageRepository.GetAll(accountId));
            Villages.Load(villages.Select(x => new ListBoxItem(x)));
        }

        public VillageTabStore VillageTabStore => _villageTabStore;
        public ReactiveCommand<Unit, Unit> LoadCurrentCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadUnloadCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadAllCommand { get; }
    }
}