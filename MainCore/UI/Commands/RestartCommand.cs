using MainCore.Common.Enums;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.UI.ViewModels.UserControls;

namespace MainCore.UI.Commands
{
    [RegisterAsTransient]
    public class RestartCommand : IRestartCommand
    {
        private readonly ITaskManager _taskManager;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        public RestartCommand(ITaskManager taskManager, WaitingOverlayViewModel waitingOverlayViewModel)
        {
            _taskManager = taskManager;
            _waitingOverlayViewModel = waitingOverlayViewModel;
        }

        public async Task Execute(int accountId)
        {
            var status = _taskManager.GetStatus(accountId);
            switch (status)
            {
                case StatusEnums.Offline:
                case StatusEnums.Starting:
                case StatusEnums.Pausing:
                case StatusEnums.Stopping:
                    ////_messageService.Show("Information", $"Account is {status}");
                    return;

                case StatusEnums.Online:
                    ////_messageService.Show("Information", $"Account should be paused first");
                    return;

                case StatusEnums.Paused:
                    await Handle(accountId);
                    return;
            }
        }

        private Task Handle(int accountId)
        {
            _taskManager.SetStatus(accountId, StatusEnums.Starting);
            _taskManager.Clear(accountId);
            _taskManager.SetStatus(accountId, StatusEnums.Online);
            return Task.CompletedTask;
        }
    }
}