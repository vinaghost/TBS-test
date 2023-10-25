using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.Services;
using MediatR;

namespace MainCore.UI.Commands
{
    public class RestartCommand : IRequest
    {
        public AccountId AccountId { get; }

        public RestartCommand(AccountId accountId)
        {
            AccountId = accountId;
        }
    }

    public class RestartCommandHandler : IRequestHandler<RestartCommand>
    {
        private readonly ITaskManager _taskManager;
        private readonly IDialogService _dialogService;

        public RestartCommandHandler(ITaskManager taskManager, IDialogService dialogService)
        {
            _taskManager = taskManager;
            _dialogService = dialogService;
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
                    _dialogService.ShowMessageBox("Information", $"Account is {status}");
                    return;

                case StatusEnums.Online:
                    _dialogService.ShowMessageBox("Information", $"Account should be paused first");
                    return;

                case StatusEnums.Paused:
                    await Handle(accountId);
                    return;
            }
        }

        private Task Handle(AccountId accountId)
        {
            _taskManager.SetStatus(accountId, StatusEnums.Starting);
            _taskManager.Clear(accountId);
            _taskManager.SetStatus(accountId, StatusEnums.Online);
            return Task.CompletedTask;
        }
    }
}