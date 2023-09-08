using FluentResults;
using MainCore.Services;
using OpenQA.Selenium;

namespace MainCore.Commands
{
    public interface IInputTextboxCommand
    {
        Task<Result> Execute(IChromeBrowser browser, By by, string content);
    }
}