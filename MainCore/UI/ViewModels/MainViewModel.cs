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
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;
        private readonly MessageBoxViewModel _messageBoxViewModel;
        private MainLayoutViewModel _mainLayoutViewModel;
        private readonly IChromeDriverInstaller _chromeDriverInstaller;
        private readonly IChromeManager _chromeManager;
        private readonly IUseragentManager _useragentManager;
        private readonly ILogService _logService;

        private readonly IAccountSettingRepository _accountSettingRepository;
        private readonly IVillageSettingRepository _villageSettingRepository;

        public MainViewModel(WaitingOverlayViewModel waitingOverlayViewModel, IDbContextFactory<AppDbContext> contextFactory, IChromeDriverInstaller chromeDriverInstaller, IChromeManager chromeManager, IUseragentManager useragentManager, ILogService logService, IAccountSettingRepository accountSettingRepository, IVillageSettingRepository villageSettingRepository, MessageBoxViewModel messageBoxViewModel)
        {
            _waitingOverlayViewModel = waitingOverlayViewModel;
            _contextFactory = contextFactory;
            _chromeDriverInstaller = chromeDriverInstaller;
            _chromeManager = chromeManager;
            _useragentManager = useragentManager;
            _logService = logService;
            _accountSettingRepository = accountSettingRepository;
            _villageSettingRepository = villageSettingRepository;
            _messageBoxViewModel = messageBoxViewModel;
        }

        public async Task Load()
        {
            //========================================//
            _waitingOverlayViewModel.Show("loading chromedriver.exe");
            try
            {
                await _chromeDriverInstaller.Install();
            }
            catch (Exception)
            {
                //await _messageBoxViewModel.Show("Error", e.Message);
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
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                //await context.Database.EnsureDeletedAsync();
                if (!await context.Database.EnsureCreatedAsync())
                {
                    var accounts = context.Accounts.AsAsyncEnumerable();
                    await foreach (var account in accounts)
                    {
                        await _accountSettingRepository.CheckSetting(account.Id, context);
                        await context.Entry(account).Collection(x => x.Villages).LoadAsync();
                        foreach (var village in account.Villages)
                        {
                            await _villageSettingRepository.CheckSetting(village.Id, context);
                        }
                    }
                }
            }

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
        public MessageBoxViewModel MessageBoxViewModel => _messageBoxViewModel;

        public MainLayoutViewModel MainLayoutViewModel
        {
            get => _mainLayoutViewModel;
            set => this.RaiseAndSetIfChanged(ref _mainLayoutViewModel, value);
        }
    }
}