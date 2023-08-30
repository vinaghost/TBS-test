using Microsoft.Extensions.DependencyInjection;
using WPFUI.Repositories;
using WPFUI.Services;
using WPFUI.ViewModels;
using WPFUI.ViewModels.Tabs;
using WPFUI.ViewModels.UserControls;

namespace WPFUI.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddUIServices(this IServiceCollection services)
        {
            services.AddSingleton<MainViewModel>();

            // Tabs
            services.AddSingleton<AddAccountViewModel>();
            services.AddSingleton<AddAccountsViewModel>();

            // UserControls
            services.AddSingleton<WaitingOverlayViewModel>();
            services.AddSingleton<MainLayoutViewModel>();

            // Repositories
            services.AddSingleton<IAccountRepository, AccountRepository>();

            // Services
            services.AddSingleton<IMessageService, MessageService>();

            return services;
        }
    }
}