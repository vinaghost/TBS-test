using FluentResults;
using HtmlAgilityPack;
using MainCore.Commands;
using MainCore.Errors;
using MainCore.Services;
using OpenQA.Selenium;

namespace NavigateCore.Commands
{
    public class SwitchTabCommand : ISwitchTabCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IWaitCommand _waitCommand;
        private readonly IClickCommand _clickCommand;

        public SwitchTabCommand(IChromeManager chromeManager, IClickCommand clickCommand, IWaitCommand waitCommand)
        {
            _chromeManager = chromeManager;
            _clickCommand = clickCommand;
            _waitCommand = waitCommand;
        }

        public async Task<Result> Execute(int accountId, int index)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var navigationBar = html.DocumentNode
                .Descendants("div")
                .FirstOrDefault(x => x.HasClass("contentNavi") && x.HasClass("subNavi"));
            if (navigationBar is null) return Result.Fail(new Retry("Cannot find tab bar"));
            var tabs = navigationBar
                .Descendants("a")
                .Where(x => x.HasClass("tabItem"))
                .ToList();
            if (navigationBar is null) return Result.Fail(new Retry("Cannot find tab buttons"));

            if (index > tabs.Count) return Result.Fail(new Retry($"Found {tabs.Count} tabs but selected tab {index}"));

            var tab = tabs[index];
            if (IsTabActive(tab)) return Result.Ok();
            Result result;
            result = await _clickCommand.Execute(chromeBrowser, By.XPath(tab.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await _waitCommand.Execute(chromeBrowser, (driver) =>
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(driver.PageSource);
                var navigationBar = doc.DocumentNode
                .Descendants("div")
                .FirstOrDefault(x => x.HasClass("contentNavi") && x.HasClass("subNavi"));
                if (navigationBar is null) return false;
                var tabs = navigationBar
                    .Descendants("a")
                    .Where(x => x.HasClass("tabItem"))
                    .ToList();
                if (tabs.Count < index) return false;
                var tab = tabs[index];
                return IsTabActive(tab);
            });
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        private bool IsTabActive(HtmlNode node)
        {
            return node.HasClass("active");
        }
    }
}