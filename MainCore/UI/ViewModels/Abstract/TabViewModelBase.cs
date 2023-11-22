using ReactiveUI;
using System.Reactive;

namespace MainCore.UI.ViewModels.Abstract
{
    public abstract class TabViewModelBase : ViewModelBase
    {
        private bool _isActive;
        private readonly ReactiveCommand<bool, Unit> TabCommand;

        public TabViewModelBase()
        {
            TabCommand = ReactiveCommand.CreateFromTask<bool>(TabTask);

            this.WhenAnyValue(x => x.IsActive)
                .InvokeCommand(TabCommand);
        }

        public bool IsActive
        {
            get => _isActive;
            set => this.RaiseAndSetIfChanged(ref _isActive, value);
        }

        private async Task TabTask(bool isActive)
        {
            if (isActive)
            {
                await OnActive();
            }
            else
            {
                await OnDeactive();
            }
        }

        protected virtual Task OnActive()
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnDeactive()
        {
            return Task.CompletedTask;
        }
    }
}