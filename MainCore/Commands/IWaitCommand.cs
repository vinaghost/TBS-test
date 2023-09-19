using FluentResults;
using MainCore.Services;
using OpenQA.Selenium;

namespace MainCore.Commands
{
    public interface IWaitCommand
    {
        Task<Result> Execute(IChromeBrowser browser, Func<IWebDriver, bool> condition);
    }
}