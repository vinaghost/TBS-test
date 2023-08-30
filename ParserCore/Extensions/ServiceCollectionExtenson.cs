using Microsoft.Extensions.DependencyInjection;
using ParserCore.Parser;

namespace ParserCore.Extensions
{
    public static class ServiceCollectionExtenson
    {
        public static IServiceCollection AddParser(this IServiceCollection services)
        {
            services.AddSingleton<ILoginPageParser, LoginPageParser>();
            return services;
        }
    }
}