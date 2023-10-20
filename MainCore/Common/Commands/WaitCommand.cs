using FluentResults;
using MainCore.Common.Errors;
using MainCore.Infrasturecture.Services;
using MediatR;
using OpenQA.Selenium;

namespace MainCore.Common.Commands
{
    public class WaitCommand : IRequest<Result>
    {
        public static Func<IWebDriver, bool> PageLoaded => driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete");

        public static Func<IWebDriver, bool> PageChanged(string part) => driver => driver.Url.Contains(part);

        public int AccountId { get; }
        public Func<IWebDriver, bool> Condition { get; }

        public WaitCommand(int accountId, Func<IWebDriver, bool> condition)
        {
            AccountId = accountId;
            Condition = condition;
        }
    }

    public class WaitCommandHandler : IRequestHandler<WaitCommand, Result>
    {
        private readonly IChromeManager _chromeManager;

        public WaitCommandHandler(IChromeManager chromeManager)
        {
            _chromeManager = chromeManager;
        }

        public async Task<Result> Handle(WaitCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;
            var chromeBrowser = _chromeManager.Get(accountId);
            var wait = chromeBrowser.Wait;

            return result = await Task.Run(() =>
            {
                try
                {
                    wait.Until(request.Condition);
                }
                catch (TimeoutException)
                {
                    return Result.Fail(new Stop("Page not loaded in 3 mins"));
                }
                return Result.Ok();
            });
        }
    }
}