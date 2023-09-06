using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;

namespace WPFUI.ViewModels.Abstract
{
    public abstract class TabViewModelBase : ViewModelBase
    {
        private bool _isActive;
        public ReactiveCommand<bool, Unit> IsActiveHandleCommand { get; }

        public TabViewModelBase()
        {
            IsActiveHandleCommand = ReactiveCommand.CreateFromTask<bool, Unit>(IsActiveHandleTask);
            IsActiveHandleCommand.ThrownExceptions.Subscribe(x => Debug.WriteLine("{0} {1}", new[] { x.Message, x.StackTrace }));

            this.WhenAnyValue(x => x.IsActive).InvokeCommand(IsActiveHandleCommand);
        }

        private async Task<Unit> IsActiveHandleTask(bool isActive)
        {
            if (isActive)
            {
                await OnActive();
            }
            else
            {
                await OnDeactive();
            }
            return Unit.Default;
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