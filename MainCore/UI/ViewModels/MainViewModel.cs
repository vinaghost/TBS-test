using MainCore.Common.Repositories;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using MainCore.UI.ViewModels.Abstract;
using MainCore.UI.ViewModels.UserControls;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;

namespace MainCore.UI.ViewModels
{
    [RegisterAsTransient(withoutInterface: true)]
    public class MainViewModel : ViewModelBase
    {
        private readonly AppDbContext _context;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;
        private readonly MessageBoxViewModel _messageBoxViewModel;
        private MainLayoutViewModel _mainLayoutViewModel;
        private readonly IChromeDriverInstaller _chromeDriverInstaller;
        private readonly IChromeManager _chromeManager;
        private readonly IUseragentManager _useragentManager;
        private readonly ILogService _logService;
        private readonly IRestClientManager _restClientManager;

        private readonly IAccountSettingRepository _accountSettingRepository;
        private readonly IVillageSettingRepository _villageSettingRepository;

        public MainViewModel(WaitingOverlayViewModel waitingOverlayViewModel, AppDbContext context, IChromeDriverInstaller chromeDriverInstaller, IChromeManager chromeManager, IUseragentManager useragentManager, ILogService logService, IAccountSettingRepository accountSettingRepository, IVillageSettingRepository villageSettingRepository, MessageBoxViewModel messageBoxViewModel, IRestClientManager restClientManager)
        {
            _waitingOverlayViewModel = waitingOverlayViewModel;
            _context = context;
            _chromeDriverInstaller = chromeDriverInstaller;
            _chromeManager = chromeManager;
            _useragentManager = useragentManager;
            _logService = logService;
            _accountSettingRepository = accountSettingRepository;
            _villageSettingRepository = villageSettingRepository;
            _messageBoxViewModel = messageBoxViewModel;
            _restClientManager = restClientManager;
        }

        public async Task Load()
        {
            bool success;
            //========================================//
            success = await _waitingOverlayViewModel.Show(
                "loading chromedriver.exe",
                _chromeDriverInstaller.Install);
            if (!success) return;

            //========================================//
            success = await _waitingOverlayViewModel.Show(
                "loading chrome's extensions",
                _chromeManager.LoadExtension
                );
            if (!success) return;
            //========================================//
            success = await _waitingOverlayViewModel.Show(
                "loading chrome's useragents",
                _useragentManager.Load);
            if (!success) return;
            //========================================//
            success = await _waitingOverlayViewModel.Show(
                "loading database",
                async () =>
                {
                    using var context = await _context.CreateDbContextAsync();

                    //await _context.Database.EnsureDeletedAsync();
                    if (!await _context.Database.EnsureCreatedAsync())
                    {
                        var accounts = _context.Accounts.AsEnumerable();
                        foreach (var account in accounts)
                        {
                            _accountSettingRepository.CheckSetting(context, account.Id);
                            _context.Entry(account).Collection(x => x.Villages).Load();
                            foreach (var village in account.Villages)
                            {
                                _villageSettingRepository.CheckSetting(context, village.Id);
                            }
                        }
                    }
                });
            if (!success) return;

            //========================================//
            success = await _waitingOverlayViewModel.Show(
                "loading log system",
                _logService.Init
                );
            if (!success) return;
            //========================================//

            success = await _waitingOverlayViewModel.Show(
                "loading user interface",
                async () =>
                {
                    await Task.Yield();
                    MainLayoutViewModel = Locator.Current.GetService<MainLayoutViewModel>();
                });
            if (!success) return;
            //========================================//
            success = await _waitingOverlayViewModel.Show(
                "loading account list",
                MainLayoutViewModel.Load
                );
            if (!success) return;
            //========================================//
        }

        public async Task Unload()
        {
            bool success;
            success = await _waitingOverlayViewModel.Show(
                "deleting proxy's cache",
                () =>
                {
                    var path = Path.Combine(App_context.BaseDirectory, "Plugins");
                    if (Directory.Exists(path)) Directory.Delete(path, true);
                });
            if (!success) return;

            success = await _waitingOverlayViewModel.Show(
                "shuting down chromedriver services",
                _chromeManager.Shutdown);
            if (!success) return;

            success = await _waitingOverlayViewModel.Show(
                "shuting down restclient services",
                _restClientManager.Shutdown);
            if (!success) return;

            success = await _waitingOverlayViewModel.Show(
                "shuting down useragent services",
                _useragentManager.Dispose);
            if (!success) return;
        }

        public WaitingOverlayViewModel WaitingOverlayViewModel => _waitingOverlayViewModel;
        public MessageBoxViewModel MessageBoxViewModel => _messageBoxViewModel;

        public MainLayoutViewModel MainLayoutViewModel
        {
            get => _mainLayoutViewModel;
            set => this.RaiseAndSetIfChanged(ref _mainLayoutViewModel, value);
        }
    }
}