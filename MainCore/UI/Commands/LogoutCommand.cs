using MainCore.Common.Commands;
using MainCore.Common.Enums;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.UI.ViewModels.UserControls;
using MediatR;

namespace MainCore.UI.Commands
{
    [RegisterAsTransient]
    public class LogoutCommand : ILogoutCommand
    {
        private readonly ITaskManager _taskManager;
        private readonly IMediator _mediator;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;
        private readonly IChromeManager _chromeManager;

        public LogoutCommand(ITaskManager taskManager, WaitingOverlayViewModel waitingOverlayViewModel, IMediator mediator, IChromeManager chromeManager)
        {
            _taskManager = taskManager;
            _waitingOverlayViewModel = waitingOverlayViewModel;
            _mediator = mediator;
            _chromeManager = chromeManager;
        }

        public async Task Execute(int accountId)
        {
            var currentTask = _taskManager.GetCurrentTask(accountId);
            if (currentTask is not null)
            {
                _taskManager.SetStatus(accountId, StatusEnums.Stopping);
                await _waitingOverlayViewModel.Show(
                    "waiting current task stops",
                    async () =>
                    {
                        var cts = _taskManager.GetCancellationTokenSource(accountId);
                        cts.Cancel();
                        await Task.Run(async () =>
                        {
                            while (currentTask.Stage != StageEnums.Waiting)
                            {
                                currentTask = _taskManager.GetCurrentTask(accountId);
                                if (currentTask is null) return;
                                await Task.Delay(500);
                            }
                        });
                    });
            }
            var chrome = _chromeManager.Get(accountId);
            await _mediator.Send(new CloseBrowserCommand(chrome));
            _taskManager.SetStatus(accountId, StatusEnums.Offline);
        }
    }
}