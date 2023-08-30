using MainCore.Services;
using OpenQA.Selenium;

namespace MainCore.Commands
{
    public interface IInputTextboxCommand
    {
        Task Execute(IChromeBrowser browser, By by, string content);
    }
}