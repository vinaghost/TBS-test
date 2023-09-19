using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;
using UpdateCore.Tasks;

namespace UpdateCore
{
    public static class ServicesConfigure
    {
        public static IServiceCollection AddUpdateServices(this IServiceCollection services)
        {
            services
                .RegisterAssemblyPublicNonGenericClasses()
                .AsPublicImplementedInterfaces(ServiceLifetime.Transient);

            services
                .AddTransient<UpdateVillageTask>();
            return services;
        }
    }
}