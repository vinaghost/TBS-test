using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WPFUI.Models.Input;
using WPFUI.Models.Validators;
using WPFUI.Repositories;
using WPFUI.Services;
using WPFUI.Stores;
using WPFUI.ViewModels;
using WPFUI.ViewModels.Tabs;
using WPFUI.ViewModels.UserControls;

namespace WPFUI
{
    public static class ServicesConfigure
    {
        public static IServiceCollection AddUIServices(this IServiceCollection services)
        {
            services.AddSingleton<MainViewModel>();

            // Tabs
            services.AddSingleton<NoAccountViewModel>();
            services.AddSingleton<AddAccountViewModel>();
            services.AddSingleton<AddAccountsViewModel>();
            services.AddSingleton<AccountSettingViewModel>();
            services.AddSingleton<EditAccountViewModel>();
            services.AddSingleton<DebugViewModel>();

            // UserControls
            services.AddSingleton<WaitingOverlayViewModel>();
            services.AddSingleton<MainLayoutViewModel>();

            // Repositories
            services.AddSingleton<IAccountRepository, AccountRepository>();
            services.AddSingleton<IAccountSettingRepository, AccountSettingRepository>();

            // Services
            services.AddSingleton<IMessageService, MessageService>();

            // Stores
            services.AddSingleton<SelectedItemStore>();
            services.AddSingleton<AccountTabStore>();

            // Validators
            services.AddSingleton<IValidator<AccountInput>, AccountInputValidator>();
            services.AddSingleton<IValidator<AccessInput>, AccessInputValidator>();

            return services;
        }
    }
}