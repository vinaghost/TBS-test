using MainCore.UI.ViewModels;
using MainCore.UI.ViewModels.UserControls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
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

            var fileDialogViewModel = Locator.Current.GetService<FileDialogViewModel>();
            fileDialogViewModel.SaveFileDialogFunc = SaveFileDialog;
            fileDialogViewModel.OpenFileDialogFunc = OpenFileDialog;
        }

        private string SaveFileDialog()
        {
            var svd = new SaveFileDialog
            {
                InitialDirectory = AppContext.BaseDirectory,
                Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
            };
            if (svd.ShowDialog() != true) return "";
            return svd.FileName;
        }

        private string OpenFileDialog()
        {
            var ofd = new OpenFileDialog
            {
                InitialDirectory = AppContext.BaseDirectory,
                Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
            };
            if (ofd.ShowDialog() != true) return "";
            return ofd.FileName;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            mainWindow.Show();
            base.OnStartup(e);
        }
    }
}