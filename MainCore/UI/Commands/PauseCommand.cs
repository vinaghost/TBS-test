using MainCore.Common.Enums;
using MainCore.Infrasturecture.Services;
using MainCore.UI.ViewModels.UserControls;
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
        private readonly MessageBoxViewModel _messageBoxViewModel;

        public PauseCommandHandler(ITaskManager taskManager, MessageBoxViewModel messageBoxViewModel)
        {
            _taskManager = taskManager;
            _messageBoxViewModel = messageBoxViewModel;
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

            await _messageBoxViewModel.Show("Information", $"Account is {status}");
        }
    }
}