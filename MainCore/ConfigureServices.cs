using FluentValidation;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using MainCore.UI.Models.Input;
using MainCore.UI.Models.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Constants = MainCore.Common.Constants;

namespace MainCore
{
    public static class DependencyInjection
    {
        private const string _connectionString = "DataSource=TBS.db;Cache=Shared";

        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite(_connectionString));

            services
                .AutoRegister(Constants.Server)
                .AddValidator();

            return services;
        }

        public static IServiceCollection AddValidator(this IServiceCollection services)
        {
            // Validators
            services
                .AddTransient<IValidator<AccountInput>, AccountInputValidator>()
                .AddTransient<IValidator<AccessInput>, AccessInputValidator>()
                .AddTransient<IValidator<AccountSettingInput>, AccountSettingInputValidator>()
                .AddTransient<IValidator<VillageSettingInput>, VillageSettingInputValidator>()
                .AddTransient<IValidator<NormalBuildInput>, NormalBuildInputValidator>()
                .AddTransient<IValidator<FarmListSettingInput>, FarmListSettingInputValidator>()
                .AddTransient<IValidator<ResourceBuildInput>, ResourceBuildInputValidator>();
            return services;
        }
    }
}