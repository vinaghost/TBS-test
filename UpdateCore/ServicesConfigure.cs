using Microsoft.Extensions.DependencyInjection;
using UpdateCore.Commands;
using UpdateCore.Parsers;

namespace UpdateCore
{
    public static class ServicesConfigure
    {
        public static IServiceCollection AddUpdateServices(this IServiceCollection services)
        {
            services.AddTransient<IVillageListParser, VillageListParser>()
                    .AddTransient<IUpdateVillageListCommand, UpdateVillageListCommand>();

            services.AddTransient<IFieldParser, FieldParser>()
                    .AddTransient<IUpdateFieldCommand, UpdateFieldCommand>();

            services.AddTransient<IInfrastructureParser, InfrastructureParser>()
                    .AddTransient<IUpdateInfrastructureCommand, UpdateInfrastructureCommand>();
            return services;
        }
    }
}