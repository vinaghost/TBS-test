using FluentResults;
using MainCore.Common.Errors;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using OpenQA.Selenium;

namespace MainCore.Common.Commands
{
    [RegisterAsTransient]
    public class ClickCommand : IClickCommand
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