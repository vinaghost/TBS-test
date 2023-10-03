using HtmlAgilityPack;
using MainCore.Entities;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace MainCore.Infrasturecture.Services
{
    public interface IChromeBrowser
    {
        string CurrentUrl { get; }
        ChromeDriver Driver { get; }
        HtmlDocument Html { get; }
        WebDriverWait Wait { get; }

        void Close();
        bool IsOpen();
        void Navigate(string url = null);
        void Setup(Access access);
        void Shutdown();
        void WaitPageLoaded();
    }
}