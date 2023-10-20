using FluentResults;
using MainCore.Infrasturecture.Services;
using OpenQA.Selenium;

namespace MainCore.Common.Commands
{
    public interface IClickCommand
    {
        Task<Result> Execute(IChromeBrowser browser, By by);
    }
}