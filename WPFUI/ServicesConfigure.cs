using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WPFUI.Commands;
using WPFUI.Models.Input;
using WPFUI.Models.Validators;
using WPFUI.Repositories;
using WPFUI.Services;
using WPFUI.Stores;
using WPFUI.ViewModels;
using WPFUI.ViewModels.Tabs;
using WPFUI.ViewModels.Tabs.Villages;
using WPFUI.ViewModels.UserControls;

namespace WPFUI
{
    public static class ServicesConfigure
    {
        public static IServiceCollection AddUIServices(this IServiceCollection services)
        {
            services
                .AddSingleton<MainViewModel>();

            // Tabs
            services
                .AddSingleton<NoAccountViewModel>()
                .AddSingleton<AddAccountViewModel>()
                .AddSingleton<AddAccountsViewModel>()
                .AddSingleton<AccountSettingViewModel>()
                .AddSingleton<VillageViewModel>()
                .AddSingleton<EditAccountViewModel>()
                .AddSingleton<DebugViewModel>();

            services
                .AddSingleton<NoVillageViewModel>()
                .AddSingleton<BuildViewModel>()
                .AddSingleton<InfoViewModel>();

            // UserControls
            services
                .AddSingleton<WaitingOverlayViewModel>()
                .AddSingleton<MainLayoutViewModel>();

            // Repositories
            services
                .AddSingleton<IAccountRepository, AccountRepository>()
                .AddSingleton<IAccountSettingRepository, AccountSettingRepository>()
                .AddSingleton<IVillageSettingRepository, VillageSettingRepository>()
                .AddSingleton<IBuildRepository, BuildRepository>();

            // Services
            services
                .AddSingleton<IMessageService, MessageService>();

            // Stores
            services
                .AddSingleton<SelectedItemStore>()
                .AddSingleton<AccountTabStore>()
                .AddSingleton<VillageTabStore>();

            // Commands
            services
                .AddTransient<ILoginCommand, LoginCommand>()
                .AddTransient<ILogoutCommand, LogoutCommand>()
                .AddTransient<IPauseCommand, PauseCommand>()
                .AddTransient<IRestartCommand, RestartCommand>();

            // Validators
            services
                .AddTransient<IValidator<AccountInput>, AccountInputValidator>()
                .AddTransient<IValidator<AccessInput>, AccessInputValidator>()
                .AddTransient<IValidator<AccountSettingInput>, AccountSettingInputValidator>()
                .AddTransient<IValidator<NormalBuildInput>, NormalBuildInputValidator>()
                .AddTransient<IValidator<ResourceBuildInput>, ResourceBuildInputValidator>();

            return services;
        }
    }
}