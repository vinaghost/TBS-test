using FluentResults;
using MainCore.Errors;
using MainCore.Services;
using OpenQA.Selenium;

namespace MainCore.Commands
{
    public class WaitCommand : IWaitCommand
    {
        private readonly IChromeManager _chromeManager;

        public WaitCommand(IChromeManager chromeManager)
        {
            _chromeManager = chromeManager;
        }

        public static Func<IWebDriver, bool> PageLoaded => driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete");

        public static Func<IWebDriver, bool> PageChanged(string part) => driver => driver.Url.Contains(part);

        public async Task<Result> Execute(int accountId, Func<IWebDriver, bool> condition)
        {
            return await Task.Run(() => ExecuteSync(accountId, condition));
        }

        private Result ExecuteSync(int accountId, Func<IWebDriver, bool> condition)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var wait = chromeBrowser.Wait;
            try
            {
                wait.Until(condition);
            }
            catch (TimeoutException)
            {
                return Result.Fail(new Stop("Page not loaded in 3 mins"));
            }
            return Result.Ok();
        }
    }
}