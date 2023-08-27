using ReactiveUI;
using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Windows;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    public class MainWindowBase : ReactiveWindow<MainViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MainWindowBase
    {
        private bool _canClose = false;
        private bool _isClosing = false;
        private bool _isLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Closing += OnClosing;

            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.MainLayoutViewModel, v => v.MainLayout.Content).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.WaitingOverlayViewModel, v => v.WaitingOverlay.Content).DisposeWith(d);
            });
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_isLoaded) return;
            _isLoaded = false;
            await ViewModel.Load();
            _isLoaded = true;
        }

        private async void OnClosing(object sender, CancelEventArgs e)
        {
            if (!_isLoaded)
            {
                e.Cancel = true;
                return;
            }

            if (_canClose) return;
            e.Cancel = true;
            if (_isClosing) return;
            _isClosing = true;

            await ViewModel.Unload();

            _canClose = true;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}