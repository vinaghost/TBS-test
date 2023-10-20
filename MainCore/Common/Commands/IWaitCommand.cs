using FluentResults;
using MainCore.Infrasturecture.Services;
using OpenQA.Selenium;

namespace MainCore.Common.Commands
{
    public interface IWaitCommand
    {
        Task<Result> Execute(IChromeBrowser browser, Func<IWebDriver, bool> condition);
    }
}