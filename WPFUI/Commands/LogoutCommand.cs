using MainCore.Commands;
using MainCore.Enums;
using MainCore.Services;
using System.Threading.Tasks;
using WPFUI.ViewModels.UserControls;

namespace WPFUI.Commands
{
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
                _waitingOverlayViewModel.Show("waiting current task stops");
                await Task.Run(async () =>
                {
                    while (currentTask.Stage != StageEnums.Waiting)
                    {
                        currentTask = _taskManager.GetCurrentTask(accountId);
                        if (currentTask is null) return;
                        await Task.Delay(500);
                    }
                });
                _waitingOverlayViewModel.Close();
            }
            await _closeBrowserCommand.Execute(accountId);
            _taskManager.SetStatus(accountId, StatusEnums.Offline);
        }
    }
}