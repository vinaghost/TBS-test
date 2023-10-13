using Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace TestProject.Mock
{
    internal class ContainerWrapper
    {
        private IServiceProvider _serviceProvider;

        public ContainerWrapper()
        {
            ServiceCollection.UseMicrosoftDependencyResolver();
        }

        public IServiceCollection ServiceCollection { get; } = new ServiceCollection();

        public IServiceProvider ServiceProvider => _serviceProvider ??= ServiceCollection.BuildServiceProvider();

        public void BuildAndUse() => ServiceProvider.UseMicrosoftDependencyResolver();
    }
}