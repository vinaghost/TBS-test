using MainCore.Common.Enums;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            IList<AutoRegisteredResult> results = null;
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite(_connectionString));

                    results = services
                        .RegisterAssemblyPublicNonGenericClasses()
                        .AsPublicImplementedInterfaces(server);
                })
                .Build();

            foreach (var item in results)
            {
                if (item.Class is null) continue;
                var result = host.Services.GetRequiredService(item.Interface);
                Assert.IsNotNull(result);
            }
        }
    }
}