using HtmlAgilityPack;
using MainCore.Models.Database;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chrome.ChromeDriverExtensions;
using OpenQA.Selenium.Support.UI;

namespace MainCore.Services
{
    public class ChromeBrowser : IChromeBrowser
    {
        private ChromeDriver _driver;
        private readonly ChromeDriverService _chromeService;
        private WebDriverWait _wait;

        private readonly string[] _extensionsPath;
        private readonly HtmlDocument _htmlDoc = new();

        private readonly string _pathUserData;

        public ChromeBrowser(string[] extensionsPath, Account account)
        {
            _pathUserData = Path.Combine(AppContext.BaseDirectory, "Data", "Cache", account.Server.Replace("https://", "").Replace("http://", "").Replace(".", "_"), account.Username);
            if (!Directory.Exists(_pathUserData)) Directory.CreateDirectory(_pathUserData);

            _extensionsPath = extensionsPath;

            _chromeService = ChromeDriverService.CreateDefaultService();
            _chromeService.HideCommandPromptWindow = true;
        }

        public void Setup(Access access)
        {
            ChromeOptions options = new();

            options.AddExtensions(_extensionsPath);

            if (!string.IsNullOrEmpty(access.ProxyHost))
            {
                if (!string.IsNullOrEmpty(access.ProxyUsername))
                {
                    options.AddHttpProxy(access.ProxyHost, access.ProxyPort, access.ProxyUsername, access.ProxyPassword);
                }
                else
                {
                    options.AddArgument($"--proxy-server={access.ProxyHost}:{access.ProxyPort}");
                }
            }

            options.AddArgument($"--user-agent={access.Useragent}");

            // So websites (Travian) can't detect the bot
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddArgument("--disable-features=UserAgentClientHint");
            options.AddArgument("--disable-logging");

            options.AddArgument("--mute-audio");

            options.AddArguments("--no-default-browser-check", "--no-first-run");
            options.AddArguments("--no-sandbox", "--test-type");

            options.AddArguments("--start-maximized");

            //if (setting.IsDontLoadImage) options.AddArguments("--blink-settings=imagesEnabled=false"); //--disable-images
            var pathUserData = Path.Combine(_pathUserData, string.IsNullOrEmpty(access.ProxyHost) ? "default" : access.ProxyHost);

            options.AddArguments($"user-data-dir={pathUserData}");

            _driver = new ChromeDriver(_chromeService, options);
            //if (setting.IsMinimized) _driver.Manage().Window.Minimize();

            _driver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(1);
            _wait = new WebDriverWait(_driver, TimeSpan.FromMinutes(3));
        }

        public ChromeDriver Driver => _driver;

        public HtmlDocument Html
        {
            get
            {
                UpdateHtml();
                return _htmlDoc;
            }
        }

        public WebDriverWait Wait => _wait;

        public void Shutdown()
        {
            Close();
            _chromeService.Dispose();
        }

        public void Close()
        {
            if (_driver is null) return;

            try
            {
                _driver.Quit();
            }
            catch { }
            _driver = null;
        }

        public bool IsOpen()
        {
            try
            {
                _ = _driver.Title;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string CurrentUrl => _driver.Url;

        public void Navigate(string url = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                Navigate(CurrentUrl);
                return;
            }

            _driver.Navigate().GoToUrl(url);
            WaitPageLoaded();
        }

        private void UpdateHtml(string source = null)
        {
            if (string.IsNullOrEmpty(source))
            {
                try
                {
                    _htmlDoc.LoadHtml(_driver.PageSource);
                }
                catch { }
            }
            else
            {
                _htmlDoc.LoadHtml(source);
            }
        }

        public void WaitPageLoaded()
        {
            try
            {
                _wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            }
            catch { }
        }
    }
}