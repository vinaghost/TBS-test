using MainCore.Common.Enums;
using MainCore.Infrasturecture.Services;
using MediatR;

namespace MainCore.UI.Commands
{
    public class LogoutCommand : IRequest
    {
        public int AccountId { get; }

        public LogoutCommand(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly ITaskManager _taskManager;
        private readonly IMediator _mediator;
        private readonly IChromeManager _chromeManager;

        public LogoutCommandHandler(IChromeManager chromeManager, IMediator mediator, ITaskManager taskManager)
        {
            _chromeManager = chromeManager;
            _mediator = mediator;
            _taskManager = taskManager;
        }

        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;

            _taskManager.SetStatus(accountId, StatusEnums.Stopping);
            await _taskManager.StopCurrentTask(accountId);

            var chrome = _chromeManager.Get(accountId);
            await chrome.Close();
            _taskManager.SetStatus(accountId, StatusEnums.Offline);
        }
    }
}