using FluentResults;
using MainCore.Services;
using OpenQA.Selenium;

namespace MainCore.Commands
{
    public interface IClickCommand
    {
        Task<Result> Execute(IChromeBrowser browser, By by);
    }
}