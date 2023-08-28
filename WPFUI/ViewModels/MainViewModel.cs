using MainCore;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;
using System.Threading.Tasks;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.UserControls;

namespace WPFUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;
        private MainLayoutViewModel _mainLayoutViewModel;

        public MainViewModel(WaitingOverlayViewModel waitingOverlayViewModel, IDbContextFactory<AppDbContext> contextFactory)
        {
            _waitingOverlayViewModel = waitingOverlayViewModel;
            _contextFactory = contextFactory;
        }

        public async Task Load()
        {
            //========================================//
            _waitingOverlayViewModel.Show("loading database");
            using var context = await _contextFactory.CreateDbContextAsync();
            //await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            //========================================//
            _waitingOverlayViewModel.Show("loading user interface");
            MainLayoutViewModel = Locator.Current.GetService<MainLayoutViewModel>();
            //========================================//
            _waitingOverlayViewModel.Show("loading account list");
            await MainLayoutViewModel.Load();
            //========================================//
            _waitingOverlayViewModel.Close();
        }

        public async Task Unload()
        {
            _waitingOverlayViewModel.Show("shutting down");
            await Task.Delay(2000);
        }

        public WaitingOverlayViewModel WaitingOverlayViewModel => _waitingOverlayViewModel;

        public MainLayoutViewModel MainLayoutViewModel
        {
            get => _mainLayoutViewModel;
            set => this.RaiseAndSetIfChanged(ref _mainLayoutViewModel, value);
        }
    }
}