using FluentResults;
using OpenQA.Selenium;

namespace MainCore.Commands
{
    public interface IWaitCommand
    {
        Task<Result> Execute(int accountId, Func<IWebDriver, bool> condition);
    }
}