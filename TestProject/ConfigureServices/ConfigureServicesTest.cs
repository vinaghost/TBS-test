using FluentAssertions;
using MainCore;
using MainCore.Common.Enums;
using MainCore.Infrasturecture.AutoRegisterDi;
using Microsoft.Extensions.DependencyInjection;

namespace TestProject.ConfigureServices
{
    [TestClass]
    public class ConfigureServicesTest
    {
        [TestMethod]
        [DataRow(ServerEnums.TravianOfficial)]
        [DataRow(ServerEnums.TTWars)]
        public void ConfigureServicesTest_ShouldPass(ServerEnums server)
        {
            var container = DependencyInjection.Setup();

            var foundServices = AutoRegisterHelpers.GetAutoRegistered(server);
            foreach (var service in foundServices)
            {
                Console.WriteLine($"{service.Interface}");
                var result = container.GetRequiredService(service.Interface);
                result.Should().NotBeNull();
            }
        }
    }
}