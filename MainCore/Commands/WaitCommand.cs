using FluentResults;
using MainCore.Errors;
using MainCore.Services;
using OpenQA.Selenium;

namespace MainCore.Commands
{
    public class WaitCommand : IWaitCommand
    {
        public static Func<IWebDriver, bool> PageLoaded => driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete");

        public static Func<IWebDriver, bool> PageChanged(string part) => driver => driver.Url.Contains(part);

        public async Task<Result> Execute(IChromeBrowser chromeBrowser, Func<IWebDriver, bool> condition)
        {
            return await Task.Run(() => ExecuteSync(chromeBrowser, condition));
        }

        private Result ExecuteSync(IChromeBrowser chromeBrowser, Func<IWebDriver, bool> condition)
        {
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