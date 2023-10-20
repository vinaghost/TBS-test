using FluentResults;
using MainCore.Common.Errors;
using MainCore.Infrasturecture.Services;
using MediatR;
using OpenQA.Selenium;

namespace MainCore.Common.Commands
{
    public class ClickCommand : IRequest<Result>
    {
        public int AccountId { get; }
        public By By { get; }

        public ClickCommand(int accountId, By by)
        {
            AccountId = accountId;
            By = by;
        }
    }

    public class ClickCommandHandler : IRequestHandler<ClickCommand, Result>
    {
        private readonly IChromeManager _chromeManager;

        public ClickCommandHandler(IChromeManager chromeManager)
        {
            _chromeManager = chromeManager;
        }

        public async Task<Result> Handle(ClickCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;
            var chromeBrowser = _chromeManager.Get(accountId);
            var chrome = chromeBrowser.Driver;

            var elements = chrome.FindElements(request.By);
            if (elements.Count == 0) return Retry.ElementNotFound();

            var element = elements[0];
            if (!element.Displayed || !element.Enabled) return Retry.ElementNotClickable();

            await Task.Run(element.Click, cancellationToken);
            return Result.Ok();
        }
    }
}