using MainCore;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using System;

namespace WPFUI
{
    public static class ServicesConfigure
    {
        public static IServiceProvider Setup()
        {
            var host = Host.CreateDefaultBuilder()
               .ConfigureServices((context, services) =>
               {
                   services.UseMicrosoftDependencyResolver();
                   var resolver = Locator.CurrentMutable;
                   resolver.InitializeSplat();
                   resolver.InitializeReactiveUI();

                   services
                       .AddCoreServices();
               })
               .Build();
            var container = host.Services;
            container.UseMicrosoftDependencyResolver();
            return container;
        }
    }
}