using FarmingCore.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;

namespace FarmingCore
{
    public static class ServicesConfigure
    {
        public static IServiceCollection AddFarmingServices(this IServiceCollection services)
        {
            services
                .RegisterAssemblyPublicNonGenericClasses()
                .AsPublicImplementedInterfaces(ServiceLifetime.Transient);
            services
                .AddTransient<UpdateFarmListTask>();
            return services;
        }
    }
}