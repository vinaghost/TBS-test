using MainCore.Commands.Base;
using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.Services;
using MediatR;

namespace MainCore.Commands.UI
{
    public class LogoutAccountByIdCommand : ByAccountIdRequestBase, IRequest
    {
        public LogoutAccountByIdCommand(AccountId accountId) : base(accountId)
        {
        }
    }

    public class LogoutAccountByIdCommandHandler : IRequestHandler<LogoutAccountByIdCommand>
    {
        private readonly ITaskManager _taskManager;
        private readonly IChromeManager _chromeManager;

        public LogoutAccountByIdCommandHandler(IChromeManager chromeManager, ITaskManager taskManager)
        {
            _chromeManager = chromeManager;
            _taskManager = taskManager;
        }

        public async Task Handle(LogoutAccountByIdCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;

            _taskManager.SetStatus(accountId, StatusEnums.Stopping);
            await _taskManager.StopCurrentTask(accountId);

            var chrome = _chromeManager.Get(accountId);
            await Task.Run(chrome.Close, cancellationToken);
            _taskManager.SetStatus(accountId, StatusEnums.Offline);
        }
    }
}