using MainCore.Common.Enums;
using MainCore.Infrasturecture.Services;
using MediatR;

namespace MainCore.UI.Commands
{
    public class PauseCommand : IRequest
    {
        public int AccountId { get; }

        public PauseCommand(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class PauseCommandHandler : IRequestHandler<PauseCommand>
    {
        private readonly ITaskManager _taskManager;
        private readonly IDialogService _dialogService;

        public PauseCommandHandler(ITaskManager taskManager, IDialogService dialogService)
        {
            _taskManager = taskManager;
            _dialogService = dialogService;
        }

        public async Task Handle(PauseCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;

            var status = _taskManager.GetStatus(accountId);
            if (status == StatusEnums.Paused)
            {
                _taskManager.SetStatus(accountId, StatusEnums.Online);
                return;
            }

            if (status == StatusEnums.Online)
            {
                await _taskManager.StopCurrentTask(accountId);
                _taskManager.SetStatus(accountId, StatusEnums.Paused);
                return;
            }

            _dialogService.ShowMessageBox("Information", $"Account is {status}");
        }
    }
}