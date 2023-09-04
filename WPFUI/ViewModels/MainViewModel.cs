using MainCore;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;
using System;
using System.IO;
using System.Threading.Tasks;
using WPFUI.Services;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.UserControls;

namespace WPFUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;
        private MainLayoutViewModel _mainLayoutViewModel;
        private readonly IMessageService _messageService;
        private readonly IChromeDriverInstaller _chromeDriverInstaller;
        private readonly IChromeManager _chromeManager;
        private readonly IUseragentManager _useragentManager;
        private readonly ILogService _logService;

        public MainViewModel(WaitingOverlayViewModel waitingOverlayViewModel, IDbContextFactory<AppDbContext> contextFactory, IChromeDriverInstaller chromeDriverInstaller, IChromeManager chromeManager, IMessageService messageService, IUseragentManager useragentManager, ILogService logService)
        {
            _waitingOverlayViewModel = waitingOverlayViewModel;
            _contextFactory = contextFactory;
            _chromeDriverInstaller = chromeDriverInstaller;
            _chromeManager = chromeManager;
            _messageService = messageService;
            _useragentManager = useragentManager;
            _logService = logService;
        }

        public async Task Load()
        {
            //========================================//
            _waitingOverlayViewModel.Show("loading chromedriver.exe");
            try
            {
                await _chromeDriverInstaller.Install();
            }
            catch (Exception e)
            {
                _messageService.Show("Error", e.Message);
                _waitingOverlayViewModel.Close();
                return;
            }
            //========================================//
            _waitingOverlayViewModel.Show("loading chrome's extensions");
            await Task.Run(_chromeManager.LoadExtension);
            //========================================//
            _waitingOverlayViewModel.Show("loading chrome's useragents");
            await _useragentManager.Load();
            //========================================//
            _waitingOverlayViewModel.Show("loading database");
            using var context = await _contextFactory.CreateDbContextAsync();
            //await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            //========================================//
            _waitingOverlayViewModel.Show("loading log system");
            _logService.Init();
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
            _waitingOverlayViewModel.Show("deleting proxy's cache");
            await Task.Run(() =>
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Plugins");
                if (Directory.Exists(path)) Directory.Delete(path, true);
            });

            _waitingOverlayViewModel.Show("shuting down chromedriver services");
            await Task.Run(_chromeManager.Shutdown);

            _useragentManager.Dispose();
        }

        public WaitingOverlayViewModel WaitingOverlayViewModel => _waitingOverlayViewModel;

        public MainLayoutViewModel MainLayoutViewModel
        {
            get => _mainLayoutViewModel;
            set => this.RaiseAndSetIfChanged(ref _mainLayoutViewModel, value);
        }
    }
}