using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;

namespace NavigateCore
{
    public static class ServicesConfigure
    {
        public static IServiceCollection AddNavigateServices(this IServiceCollection services)
        {
            services
                .RegisterAssemblyPublicNonGenericClasses()
                .AsPublicImplementedInterfaces(ServiceLifetime.Transient);
            return services;
        }
    }
}