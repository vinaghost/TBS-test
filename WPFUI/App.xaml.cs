﻿using MainCore;
using MainCore.Infrasturecture.Services;
using MainCore.UI.ViewModels;
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
            Container = DependencyInjection.Setup();

            mainWindow = new MainWindow()
            {
                ViewModel = Locator.Current.GetService<MainViewModel>(),
            };

            var dialogService = Locator.Current.GetService<IDialogService>() as DialogService;
            dialogService.MessageBoxFunc = ShowMessage;
            dialogService.ConfirmBoxFunc = ShowConfirm;
            dialogService.SaveFileDialogFunc = SaveFileDialog;
            dialogService.OpenFileDialogFunc = OpenFileDialog;
        }

        private void ShowMessage(string message, string title)
        {
            MessageBox.Show(message, title);
        }

        private bool ShowConfirm(string message, string title)
        {
            var answer = MessageBox.Show(message, title, MessageBoxButton.YesNo);
            return answer == MessageBoxResult.Yes;
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