using MainCore.Repositories;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WPFUI.Enums;
using WPFUI.Models.Output;
using WPFUI.Stores;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs
{
    public class VillageViewModel : AccountTabViewModelBase
    {
        private readonly IVillageRepository _villageRepository;
        private readonly VillageTabStore _villageTabStore;

        public VillageViewModel(IVillageRepository villageRepository, VillageTabStore villageTabStore)
        {
            _villageRepository = villageRepository;
            _villageTabStore = villageTabStore;

            var villageObservable = this.WhenAnyValue(x => x.SelectedVillage);
            villageObservable.BindTo(_selectedItemStore, vm => vm.Village);
            villageObservable.Subscribe(x =>
            {
                var tabType = VillageTabType.Normal;
                if (x is null) tabType = VillageTabType.NoVillage;
                _villageTabStore.SetTabType(tabType);
            });
        }

        protected override async Task Load(int accountId)
        {
            await LoadVillageList(accountId);
        }

        private async Task LoadVillageList(int accountId)
        {
            var villages = await _villageRepository.Get(accountId);
            Villages.Clear();
            foreach (var village in villages)
            {
                Villages.Add(new(village));
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
        private ListBoxItem _selectedVIllage;

        public ListBoxItem SelectedVillage
        {
            get => _selectedVIllage;
            set => this.RaiseAndSetIfChanged(ref _selectedVIllage, value);
        }

        public VillageTabStore VillageTabStore => _villageTabStore;
    }
}