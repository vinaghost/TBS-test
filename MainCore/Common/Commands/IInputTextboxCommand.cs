using FluentResults;
using MainCore.Infrasturecture.Services;
using OpenQA.Selenium;

namespace MainCore.Common.Commands
{
    public interface IInputTextboxCommand
    {
        Task<Result> Execute(IChromeBrowser browser, By by, string content);
    }
}