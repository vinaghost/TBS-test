using FluentResults;
using MainCore.Common.Errors;
using MainCore.Infrasturecture.Services;
using MediatR;
using OpenQA.Selenium;

namespace MainCore.Common.Commands
{
    public class InputTextBoxCommand : IRequest<Result>
    {
        public int AccountId { get; }
        public By By { get; }
        public string Content { get; }

        public InputTextBoxCommand(int accountId, By by, string content)
        {
            AccountId = accountId;
            By = by;
            Content = content;
        }
    }

    public class InputTextBoxCommandHandler : IRequestHandler<InputTextBoxCommand, Result>
    {
        private readonly IChromeManager _chromeManager;

        public InputTextBoxCommandHandler(IChromeManager chromeManager)
        {
            _chromeManager = chromeManager;
        }

        public async Task<Result> Handle(InputTextBoxCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;
            var chromeBrowser = _chromeManager.Get(accountId);
            var chrome = chromeBrowser.Driver;

            var elements = chrome.FindElements(request.By);
            if (elements.Count == 0) return Retry.ElementNotFound();

            var element = elements[0];
            if (!element.Displayed || !element.Enabled) return Retry.ElementNotClickable();

            await Task.Run(() =>
            {
                element.SendKeys(Keys.Home);
                element.SendKeys(Keys.Shift + Keys.End);
                element.SendKeys(request.Content);
            }, cancellationToken);
            return Result.Ok();
        }
    }
}