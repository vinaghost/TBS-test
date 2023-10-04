using MainCore.Common.Enums;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TestProject.ConfigureServices
{
    [TestClass]
    public class ConfigureServicesCoreTest
    {
        private const string _connectionString = "DataSource=TBS.db;Cache=Shared";

        [DataTestMethod]
        [DataRow(ServerEnums.TravianOfficial)]
        [DataRow(ServerEnums.TTWars)]
        public void ConfigureServicesTest(ServerEnums server)
        {
            var services = new ServiceCollection();
            services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite(_connectionString));

            var results = services
                .RegisterAssemblyPublicNonGenericClasses(typeof(AppDbContext).Assembly)
                .AsPublicImplementedInterfaces(server);
            var provider = services.BuildServiceProvider();
            foreach (var item in results)
            {
                if (item.Class is null) continue;
                var result = provider.GetRequiredService(item.Interface);
                Assert.IsNotNull(result);
            }
        }
    }
}