using MainCore.Services;
using OpenQA.Selenium;

namespace MainCore.Commands
{
    public interface IClickButtonCommand
    {
        Task Execute(IChromeBrowser browser, By by);
    }
}