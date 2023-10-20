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
                this.BindCommand(ViewModel, vm => vm.OkCommand, v => v.OK).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.YesCommand, v => v.Yes).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.NoCommand, v => v.No).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.DialogYesNo, v => v.YesNo.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.DialogOk, v => v.OK.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.DialogShown, v => v.Grid.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.DialogHeader, v => v.Header.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.DialogMessage, v => v.Message.Text).DisposeWith(d);
            });
        }

        private bool NegativeValue(bool val)
        {
            return !val;
        }
    }
}