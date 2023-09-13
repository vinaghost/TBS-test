using MainCore.Repositories;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WPFUI.Models.Output;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class BuildViewModel : VillageTabViewModelBase
    {
        private readonly IBuildingRepository _buildingRepository;
        private readonly IJobRepository _jobRepository;

        public BuildViewModel(IBuildingRepository buildingRepository, IJobRepository jobRepository)
        {
            _buildingRepository = buildingRepository;
            _jobRepository = jobRepository;

            _buildingRepository.BuildingUpdated += BuildingUpdated;
        }

        private async Task BuildingUpdated(int villageId)
        {
            if (!IsActive) return;
            if (villageId != VillageId) return;
            await Observable.StartAsync(() => LoadBuildings(villageId), RxApp.MainThreadScheduler);
        }

        protected override async Task Load(int villageId)
        {
            await LoadBuildings(villageId);
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
                SelectedBuilding = Jobs[0];
            }
            else
            {
                SelectedBuilding = null;
            }
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