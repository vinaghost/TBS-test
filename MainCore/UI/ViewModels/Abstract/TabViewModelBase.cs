using ReactiveUI;

namespace MainCore.UI.ViewModels.Abstract
{
    public abstract class TabViewModelBase : ViewModelBase
    {
        private bool _isActive;

        public TabViewModelBase()
        {
            this.WhenAnyValue(x => x.IsActive).Subscribe(async x =>
            {
                await IsActiveHandleTask(x);
            });
        }

        private async Task IsActiveHandleTask(bool isActive)
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

        public bool IsActive
        {
            get => _isActive;
            set => this.RaiseAndSetIfChanged(ref _isActive, value);
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