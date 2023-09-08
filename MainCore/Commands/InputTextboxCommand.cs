using FluentResults;
using MainCore.Errors;
using MainCore.Services;
using OpenQA.Selenium;

namespace MainCore.Commands
{
    public class InputTextboxCommand : IInputTextboxCommand
    {
        public async Task<Result> Execute(IChromeBrowser browser, By by, string content)
        {
            var chrome = browser.Driver;
            var elements = chrome.FindElements(by);
            if (elements.Count == 0) return Retry.ElementNotFound();

            var element = elements[0];
            if (!element.Displayed || !element.Enabled) return Retry.ElementNotClickable();

            await Task.Run(() =>
            {
                element.SendKeys(Keys.Home);
                element.SendKeys(Keys.Shift + Keys.End);
                element.SendKeys(content);
            });
            return Result.Ok();
        }
    }
}