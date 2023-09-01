using MainCore.Commands;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

            // commands
            services.AddSingleton<IOpenBrowserCommand, OpenBrowserCommand>();
            services.AddSingleton<ICloseBrowserCommand, CloseBrowserCommand>();
            services.AddSingleton<IInputTextboxCommand, InputTextboxCommand>();
            services.AddSingleton<IClickButtonCommand, ClickButtonCommand>();
            return services;
        }
    }
}