using FluentResults;
using HtmlAgilityPack;
using MainCore.Common.Errors;
using MainCore.Entities;
using MainCore.Features.Navigate.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using OpenQA.Selenium;

namespace MainCore.Features.Navigate.Commands
{
    [RegisterAsTransient]
    public class SwitchTabCommand : ISwitchTabCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly INavigationTabParser _navigationTabParser;

        public SwitchTabCommand(IChromeManager chromeManager, INavigationTabParser navigationTabParser)
        {
            _chromeManager = chromeManager;
            _navigationTabParser = navigationTabParser;
        }

        public async Task<Result> Execute(AccountId accountId, int index)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;

            var count = _navigationTabParser.CountTab(html);
            if (index > count) return Result.Fail(new Retry($"Found {count} tabs but selected tab {index}"));
            var tab = _navigationTabParser.GetTab(html, index);
            if (_navigationTabParser.IsTabActive(tab)) return Result.Ok();

            Result result;
            result = await chromeBrowser.Click(By.XPath(tab.XPath));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            bool tabActived(IWebDriver driver)
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(driver.PageSource);
                var count = _navigationTabParser.CountTab(doc);
                if (index > count) return false;
                var tab = _navigationTabParser.GetTab(doc, index);
                if (!_navigationTabParser.IsTabActive(tab)) return false;
                return true;
            };

            result = await chromeBrowser.Wait(tabActived);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }
    }
}