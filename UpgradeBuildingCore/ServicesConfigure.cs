using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;
using UpgradeBuildingCore.Tasks;

namespace UpgradeBuildingCore
{
    public static class ServicesConfigure
    {
        public static IServiceCollection AddUpgradeBuildingServices(this IServiceCollection services)
        {
            services
                .RegisterAssemblyPublicNonGenericClasses()
                .AsPublicImplementedInterfaces(ServiceLifetime.Transient);

            services
                .AddTransient<UpgradeBuildingTask>();
            return services;
        }
    }
}