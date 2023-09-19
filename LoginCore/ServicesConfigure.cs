using LoginCore.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;

namespace LoginCore
{
    public static class ServicesConfigure
    {
        public static IServiceCollection AddLoginServices(this IServiceCollection services)
        {
            services
                .RegisterAssemblyPublicNonGenericClasses()
                .AsPublicImplementedInterfaces(ServiceLifetime.Transient);
            services
                .AddTransient<LoginTask>();

            return services;
        }
    }
}