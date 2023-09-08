using LoginCore.Commands;
using LoginCore.Parser;
using LoginCore.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace LoginCore
{
    public static class ServicesConfigure
    {
        public static IServiceCollection AddLoginServices(this IServiceCollection services)
        {
            services.AddTransient<ILoginPageParser, LoginPageParser>();
            services.AddTransient<ILoginCommand, LoginCommand>();
            services.AddTransient<LoginTask>();

            return services;
        }
    }
}