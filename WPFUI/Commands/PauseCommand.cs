using MainCore.Common.Enums;
using MainCore.Infrasturecture.Services;
using System.Threading.Tasks;
using WPFUI.Services;
using WPFUI.ViewModels.UserControls;

namespace WPFUI.Commands
{
    public class PauseCommand : IPauseCommand
    {
        private readonly ITaskManager _taskManager;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;
        private readonly IMessageService _messageService;

        public PauseCommand(ITaskManager taskManager, WaitingOverlayViewModel waitingOverlayViewModel, IMessageService messageService)
        {
            _taskManager = taskManager;
            _waitingOverlayViewModel = waitingOverlayViewModel;
            _messageService = messageService;
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
                    var cts = _taskManager.GetCancellationTokenSource(accountId);
                    _taskManager.SetStatus(accountId, StatusEnums.Pausing);
                    _waitingOverlayViewModel.Show("waiting current task stops");
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
                    _waitingOverlayViewModel.Close();
                }

                _taskManager.SetStatus(accountId, StatusEnums.Paused);
                return;
            }

            _messageService.Show("Information", $"Account is {status}");
        }
    }
}