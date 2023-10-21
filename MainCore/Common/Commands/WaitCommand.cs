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

        public IChromeBrowser Browser { get; }
        public Func<IWebDriver, bool> Condition { get; }

        public WaitCommand(IChromeBrowser browser, Func<IWebDriver, bool> condition)
        {
            Browser = browser;
            Condition = condition;
        }
    }

    public class WaitCommandHandler : IRequestHandler<WaitCommand, Result>
    {
        public async Task<Result> Handle(WaitCommand request, CancellationToken cancellationToken)
        {
            var wait = request.Browser.Wait;
            return await Task.Run(() =>
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