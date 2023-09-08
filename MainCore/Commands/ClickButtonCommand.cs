using FluentResults;
using MainCore.Errors;
using MainCore.Services;
using OpenQA.Selenium;

namespace MainCore.Commands
{
    public class ClickButtonCommand : IClickButtonCommand
    {
        public async Task<Result> Execute(IChromeBrowser browser, By by)
        {
            var chrome = browser.Driver;
            var elements = chrome.FindElements(by);
            if (elements.Count == 0) return Retry.ElementNotFound();
            var element = elements[0];
            if (!element.Displayed || !element.Enabled) return Retry.ElementNotClickable();

            await Task.Run(element.Click);

            return Result.Ok();
        }
    }
}