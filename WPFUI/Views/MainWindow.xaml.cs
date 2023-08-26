using ReactiveUI;
using System;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}