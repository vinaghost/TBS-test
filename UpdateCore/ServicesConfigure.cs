using Microsoft.Extensions.DependencyInjection;
using UpdateCore.Commands;
using UpdateCore.Parsers;

namespace UpdateCore
{
    public static class ServicesConfigure
    {
        public static IServiceCollection AddUpdateServices(this IServiceCollection services)
        {
            services.AddTransient<IVillageListParser, VillageListParser>();
            services.AddTransient<IUpdateVillageListCommand, UpdateVillageListCommand>();
            return services;
        }
    }
}