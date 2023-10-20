using FluentResults;
using MainCore.Infrasturecture.Services;
using MediatR;

namespace MainCore.Common.Commands
{
    public class CloseBrowserCommand : IRequest<Result>
    {
        public int AccountId { get; }

        public CloseBrowserCommand(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class CloseBrowserCommandHandler : IRequestHandler<CloseBrowserCommand, Result>
    {
        private readonly IChromeManager _chromeManager;

        public CloseBrowserCommandHandler(IChromeManager chromeManager)
        {
            _chromeManager = chromeManager;
        }

        public async Task<Result> Handle(CloseBrowserCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;
            var chromeBrowser = _chromeManager.Get(accountId);
            var chrome = chromeBrowser.Driver;

            await Task.Run(chrome.Close, cancellationToken);
            return Result.Ok();
        }
    }
}