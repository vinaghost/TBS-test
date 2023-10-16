using MainCore.Common.Enums;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.UI.ViewModels.UserControls;

namespace MainCore.UI.Commands
{
    [RegisterAsTransient]
    public class PauseCommand : IPauseCommand
    {
        private readonly ITaskManager _taskManager;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;
        private readonly MessageBoxViewModel _messageBoxViewModel;

        public PauseCommand(ITaskManager taskManager, WaitingOverlayViewModel waitingOverlayViewModel, MessageBoxViewModel messageBoxViewModel)
        {
            _taskManager = taskManager;
            _waitingOverlayViewModel = waitingOverlayViewModel;
            _messageBoxViewModel = messageBoxViewModel;
        }

        public async Task Execute(int accountId)
        {
            var status = _taskManager.GetStatus(accountId);
            if (status == StatusEnums.Paused)
            {
                _taskManager.SetStatus(accountId, StatusEnums.Online);
                return;
            }

            if (status == StatusEnums.Online)
            {
                var currentTask = _taskManager.GetCurrentTask(accountId);
                if (currentTask is not null)
                {
                    _taskManager.SetStatus(accountId, StatusEnums.Pausing);
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

                _taskManager.SetStatus(accountId, StatusEnums.Paused);
                return;
            }

            await _messageBoxViewModel.Show("Information", $"Account is {status}");
        }
    }
}