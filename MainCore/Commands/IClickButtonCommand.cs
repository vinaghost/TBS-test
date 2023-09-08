using FluentResults;
using MainCore.Services;
using OpenQA.Selenium;

namespace MainCore.Commands
{
    public interface IClickButtonCommand
    {
        Task<Result> Execute(IChromeBrowser browser, By by);
    }
}