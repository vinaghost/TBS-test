using FluentResults;
using HtmlAgilityPack;
using MainCore.Entities;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace MainCore.Infrasturecture.Services
{
    public interface IChromeBrowser
    {
        string CurrentUrl { get; }
        ChromeDriver Driver { get; }
        HtmlDocument Html { get; }

        Task<Result> Click(By by);

        Task Close();

        Task<Result> InputTextbox(By by, string content);

        bool IsOpen();

        Task Navigate(string url = null);

        void Setup(Access access);

        void Shutdown();

        Task<Result> Wait(Func<IWebDriver, bool> condition);

        Task<Result> WaitPageChanged(string part);

        Task<Result> WaitPageLoaded();
    }
}