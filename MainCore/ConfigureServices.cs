using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Constants = MainCore.Common.Constants;

namespace MainCore
{
    public static class DependencyInjection
    {
        private const string _connectionString = "DataSource=TBS.db;Cache=Shared";

        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite(_connectionString));

            services
                .RegisterAssemblyPublicNonGenericClasses()
                .AsPublicImplementedInterfaces(Constants.Server);

            return services;
        }
    }
}