using MainCore.Commands;
using MainCore.Repositories;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;

namespace MainCore
{
    public static class ServicesConfigure
    {
        private const string _connectionString = "DataSource=TBS.db;Cache=Shared";

        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite(_connectionString));

            // services
            services.AddSingleton<IChromeDriverInstaller, ChromeDriverInstaller>()
                    .AddSingleton<IChromeManager, ChromeManager>()
                    .AddSingleton<IUseragentManager, UseragentManager>()
                    .AddSingleton<ITimerManager, TimerManager>()
                    .AddSingleton<ITaskManager, TaskManager>()
                    .AddSingleton<ILogService, LogService>();

            services.AddSingleton<ILogEventSink, LogSink>();

            // repositories
            services.AddSingleton<IAccountSettingRepository, AccountSettingRepository>()
                    .AddSingleton<IVillageRepository, VillageRepository>()
                    .AddSingleton<IBuildingRepository, BuildingRepository>();

            // commands
            services.AddTransient<IOpenBrowserCommand, OpenBrowserCommand>()
                    .AddTransient<ICloseBrowserCommand, CloseBrowserCommand>()
                    .AddTransient<IInputTextboxCommand, InputTextboxCommand>()
                    .AddTransient<IClickButtonCommand, ClickButtonCommand>()
                    .AddTransient<IWaitCommand, WaitCommand>();
            return services;
        }
    }
}