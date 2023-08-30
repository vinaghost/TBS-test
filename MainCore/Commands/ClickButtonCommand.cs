using MainCore.Services;
using OpenQA.Selenium;

namespace MainCore.Commands
{
    public class ClickButtonCommand : IClickButtonCommand
    {
        public async Task Execute(IChromeBrowser browser, By by)
        {
            var chrome = browser.Driver;
            var elements = chrome.FindElements(by);
            if (elements.Count == 0)
            {
                return;
            }
            var element = elements[0];
            if (!element.Displayed || !element.Enabled) return;

            await Task.Run(element.Click);
        }
    }
}