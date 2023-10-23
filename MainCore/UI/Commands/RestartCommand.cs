using MainCore.Common.Enums;
using MainCore.Infrasturecture.Services;
using MainCore.UI.ViewModels.UserControls;
using MediatR;

namespace MainCore.UI.Commands
{
    public class RestartCommand : IRequest
    {
        public int AccountId { get; }

        public RestartCommand(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class RestartCommandHandler : IRequestHandler<RestartCommand>
    {
        private readonly ITaskManager _taskManager;
        private readonly MessageBoxViewModel _messageBoxViewModel;

        public RestartCommandHandler(ITaskManager taskManager, MessageBoxViewModel messageBoxViewModel)
        {
            _taskManager = taskManager;
            _messageBoxViewModel = messageBoxViewModel;
        }

        public async Task Handle(RestartCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;

            var status = _taskManager.GetStatus(accountId);

            switch (status)
            {
                case StatusEnums.Offline:
                case StatusEnums.Starting:
                case StatusEnums.Pausing:
                case StatusEnums.Stopping:
                    await _messageBoxViewModel.Show("Information", $"Account is {status}");
                    return;

                case StatusEnums.Online:
                    await _messageBoxViewModel.Show("Information", $"Account should be paused first");
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