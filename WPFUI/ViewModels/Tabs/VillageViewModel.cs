using MainCore.Common.Repositories;
using MainCore.Features.Update.Tasks;
using MainCore.Infrasturecture.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WPFUI.Enums;
using WPFUI.Models.Output;
using WPFUI.Services;
using WPFUI.Stores;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs
{
    public class VillageViewModel : AccountTabViewModelBase
    {
        private readonly IVillageRepository _villageRepository;
        private readonly VillageTabStore _villageTabStore;
        private readonly IMessageService _messageService;
        private readonly ITaskManager _taskManager;

        public VillageViewModel(IVillageRepository villageRepository, VillageTabStore villageTabStore, IMessageService messageService, ITaskManager taskManager)
        {
            _villageRepository = villageRepository;
            _villageTabStore = villageTabStore;
            _messageService = messageService;
            _taskManager = taskManager;

            LoadCurrentCommand = ReactiveCommand.CreateFromTask(LoadCurrentTask);
            LoadUnloadCommand = ReactiveCommand.CreateFromTask(LoadUnloadTask);
            LoadAllCommand = ReactiveCommand.CreateFromTask(LoadAllTask);

            _villageRepository.VillageListChanged += VillageListChanged;

            var villageObservable = this.WhenAnyValue(x => x.SelectedVillage);
            villageObservable.BindTo(_selectedItemStore, vm => vm.Village);
            villageObservable.Subscribe(x =>
            {
                var tabType = VillageTabType.Normal;
                if (x is null) tabType = VillageTabType.NoVillage;
                _villageTabStore.SetTabType(tabType);
            });
        }

        private async Task VillageListChanged(int accountId)
        {
            if (!IsActive) return;
            if (accountId != AccountId) return;
            await Observable.Start(async () => await LoadVillageList(accountId), RxApp.MainThreadScheduler);
        }

        private async Task LoadCurrentTask()
        {
            if (SelectedVillage is null)
            {
                _messageService.Show("Warning", "No village selected");
                return;
            }

            await Task.Run(() => _taskManager.Add<UpdateVillageTask>(AccountId, SelectedVillage.Id));
            _messageService.Show("Information", $"Added update task");
            return;
        }

        private async Task LoadUnloadTask()
        {
            if (SelectedVillage is null)
            {
                _messageService.Show("Warning", "No village selected");
                return;
            }

            var villages = await _villageRepository.GetUnloadList(AccountId);
            foreach (var village in villages)
            {
                _taskManager.Add<UpdateVillageTask>(AccountId, village.Id);
            }
            _messageService.Show("Information", $"Added update task");
            return;
        }

        private async Task LoadAllTask()
        {
            if (SelectedVillage is null)
            {
                _messageService.Show("Warning", "No village selected");
                return;
            }

            var villages = await _villageRepository.GetList(AccountId);
            foreach (var village in villages)
            {
                _taskManager.Add<UpdateVillageTask>(AccountId, village.Id);
            }
            _messageService.Show("Information", $"Added update task");
            return;
        }

        protected override async Task Load(int accountId)
        {
            await LoadVillageList(accountId);
        }

        private async Task LoadVillageList(int accountId)
        {
            var villages = await _villageRepository.GetList(accountId);
            Villages.Clear();
            foreach (var village in villages)
            {
                //for (var i = 0; i < 40; i++)
                //{
                //    village.Name = $"{i}";
                Villages.Add(new(village));
                //}
            }

            if (villages.Count > 0)
            {
                SelectedVillage = Villages[0];
            }
            else
            {
                SelectedVillage = null;
            }
        }

        public ObservableCollection<ListBoxItem> Villages { get; } = new();
        private ListBoxItem _selectedVillage;

        public ListBoxItem SelectedVillage
        {
            get => _selectedVillage;
            set => this.RaiseAndSetIfChanged(ref _selectedVillage, value);
        }

        public VillageTabStore VillageTabStore => _villageTabStore;
        public ReactiveCommand<Unit, Unit> LoadCurrentCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadUnloadCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadAllCommand { get; }
    }
}