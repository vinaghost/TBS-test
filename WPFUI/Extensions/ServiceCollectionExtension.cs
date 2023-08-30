﻿using MainCore;
using MainCore.Commands;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ParserCore.Parser;
using WPFUI.Repositories;
using WPFUI.Services;
using WPFUI.ViewModels;
using WPFUI.ViewModels.Tabs;
using WPFUI.ViewModels.UserControls;

namespace WPFUI.Extensions
{
    public static class ServiceCollectionExtension
    {
        private const string _connectionString = "DataSource=TBS.db;Cache=Shared";

        public static IServiceCollection AddLogicServices(this IServiceCollection services)
        {
            services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite(_connectionString));
            return services;
        }

        public static IServiceCollection AddCoreSerivce(this IServiceCollection services)
        {
            services.AddSingleton<IChromeDriverInstaller, ChromeDriverInstaller>();
            services.AddSingleton<IChromeManager, ChromeManager>();
            services.AddSingleton<IUseragentManager, UseragentManager>();

            return services;
        }

        public static IServiceCollection AddParser(this IServiceCollection services)
        {
            services.AddSingleton<ILoginPageParser, LoginPageParser>();
            return services;
        }

        public static IServiceCollection AddCommand(this IServiceCollection services)
        {
            services.AddSingleton<ILoginCommand, LoginCommand>();
            services.AddSingleton<ILogoutCommand, LogoutCommand>();
            services.AddSingleton<IInputTextboxCommand, InputTextboxCommand>();
            services.AddSingleton<IClickButtonCommand, ClickButtonCommand>();
            return services;
        }

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