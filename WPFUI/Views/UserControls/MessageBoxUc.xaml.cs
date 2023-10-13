using MainCore.UI.ViewModels.UserControls;
using ReactiveUI;
using System.Reactive.Disposables;

namespace WPFUI.Views.UserControls
{
    public class MessageBoxUcBase : ReactiveUserControl<MessageBoxViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for MessageBoxUc.xaml
    /// </summary>
    public partial class MessageBoxUc : MessageBoxUcBase
    {
        public MessageBoxUc()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.IsEnable, v => v.Grid.Visibility).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Title, v => v.Title.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Message, v => v.Message.Text).DisposeWith(d);
            });
        }
    }
}