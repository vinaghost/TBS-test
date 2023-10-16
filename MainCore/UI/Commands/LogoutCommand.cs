using MainCore.Common.Commands;
using MainCore.Common.Enums;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.UI.ViewModels.UserControls;

namespace MainCore.UI.Commands
{
    [RegisterAsTransient]
    public class LogoutCommand : ILogoutCommand
    {
        private readonly ITaskManager _taskManager;
        private readonly ICloseBrowserCommand _closeBrowserCommand;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        public LogoutCommand(ITaskManager taskManager, ICloseBrowserCommand closeBrowserCommand, WaitingOverlayViewModel waitingOverlayViewModel)
        {
            _taskManager = taskManager;
            _closeBrowserCommand = closeBrowserCommand;
            _waitingOverlayViewModel = waitingOverlayViewModel;
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
            await _closeBrowserCommand.Execute(accountId);
            _taskManager.SetStatus(accountId, StatusEnums.Offline);
        }
    }
}