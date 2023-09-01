using LoginCore.Commands;
using LoginCore.Parser;
using Microsoft.Extensions.DependencyInjection;

namespace LoginCore
{
    public static class ServicesConfigure
    {
        public static IServiceCollection AddLoginServices(this IServiceCollection services)
        {
            services.AddSingleton<ILoginPageParser, LoginPageParser>();
            services.AddSingleton<ILoginCommand, LoginCommand>();
            return services;
        }
    }
}