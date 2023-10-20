using FluentResults;
using HtmlAgilityPack;
using MainCore.Common.Commands;
using MainCore.Common.Errors;
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
        private readonly IWaitCommand _waitCommand;
        private readonly IClickCommand _clickCommand;
        private readonly INavigationTabParser _navigationTabParser;

        public SwitchTabCommand(IChromeManager chromeManager, IClickCommand clickCommand, IWaitCommand waitCommand, INavigationTabParser navigationTabParser)
        {
            _chromeManager = chromeManager;
            _clickCommand = clickCommand;
            _waitCommand = waitCommand;
            _navigationTabParser = navigationTabParser;
        }

        public async Task<Result> Execute(IChromeBrowser chromeBrowser, int index)
        {
            var html = chromeBrowser.Html;
            var count = _navigationTabParser.CountTab(html);

            if (index > count) return Result.Fail(new Retry($"Found {count} tabs but selected tab {index}"));

            var tab = _navigationTabParser.GetTab(html, index);
            if (_navigationTabParser.IsTabActive(tab)) return Result.Ok();
            Result result;
            result = await _clickCommand.Execute(chromeBrowser, By.XPath(tab.XPath));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            var tabActived = new Func<IWebDriver, bool>(driver =>
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(driver.PageSource);
                var count = _navigationTabParser.CountTab(doc);
                if (index > count) return false;
                var tab = _navigationTabParser.GetTab(doc, index);
                if (!_navigationTabParser.IsTabActive(tab)) return false;
                return true;
            });

            result = await _waitCommand.Execute(chromeBrowser, tabActived);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }

        public async Task<Result> Execute(int accountId, int index)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            return await Execute(chromeBrowser, index);
        }
    }
}