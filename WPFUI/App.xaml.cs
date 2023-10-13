using MainCore.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Splat;
using System;
using System.Windows;
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
            Container = ServicesConfigure.Setup();

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