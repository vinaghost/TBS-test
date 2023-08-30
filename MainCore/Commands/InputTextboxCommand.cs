using MainCore.Services;
using OpenQA.Selenium;

namespace MainCore.Commands
{
    public class InputTextboxCommand : IInputTextboxCommand
    {
        public async Task Execute(IChromeBrowser browser, By by, string content)
        {
            var chrome = browser.Driver;
            var elements = chrome.FindElements(by);
            if (elements.Count == 0)
            {
                return;
            }
            var element = elements[0];
            await Task.Run(() =>
            {
                element.SendKeys(Keys.Home);
                element.SendKeys(Keys.Shift + Keys.End);
                element.SendKeys(content);
            });
        }
    }
}