using MainCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using WPFUI.ViewModels;
using WPFUI.Views;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly MainWindow mainWindow;

        public IServiceProvider Container { get; private set; }

        public App()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.UseMicrosoftDependencyResolver();
                    var resolver = Locator.CurrentMutable;
                    resolver.InitializeSplat();
                    resolver.InitializeReactiveUI();

                    services
                        .AddCoreServices()
                        .AddUIServices();
                })
                .Build();
            Container = host.Services;
            Container.UseMicrosoftDependencyResolver();

            mainWindow = new MainWindow()
            {
                ViewModel = Locator.Current.GetService<MainViewModel>(),
            };
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            mainWindow.Show();
            base.OnStartup(e);
        }
    }
}