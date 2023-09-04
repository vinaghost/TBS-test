using MainCore.Commands;
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
            services.AddSingleton<IChromeDriverInstaller, ChromeDriverInstaller>();
            services.AddSingleton<IChromeManager, ChromeManager>();
            services.AddSingleton<IUseragentManager, UseragentManager>();
            services.AddSingleton<ITimerManager, TimerManager>();
            services.AddSingleton<ITaskManager, TaskManager>();
            services.AddSingleton<ILogService, LogService>();

            services.AddSingleton<ILogEventSink, LogSink>();

            // commands
            services.AddTransient<IOpenBrowserCommand, OpenBrowserCommand>();
            services.AddTransient<ICloseBrowserCommand, CloseBrowserCommand>();
            services.AddTransient<IInputTextboxCommand, InputTextboxCommand>();
            services.AddTransient<IClickButtonCommand, ClickButtonCommand>();
            return services;
        }
    }
}