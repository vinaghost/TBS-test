using FluentResults;
using HtmlAgilityPack;
using MainCore.Common.Commands;
using MainCore.Common.Errors;
using MainCore.Features.Navigate.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MediatR;
using OpenQA.Selenium;

namespace MainCore.Features.Navigate.Commands
{
    [RegisterAsTransient]
    public class SwitchTabCommand : ISwitchTabCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly INavigationTabParser _navigationTabParser;
        private readonly IMediator _mediator;

        public SwitchTabCommand(IChromeManager chromeManager, INavigationTabParser navigationTabParser, IMediator mediator)
        {
            _chromeManager = chromeManager;
            _navigationTabParser = navigationTabParser;
            _mediator = mediator;
        }

        public async Task<Result> Execute(IChromeBrowser chromeBrowser, int index)
        {
            var html = chromeBrowser.Html;
            var count = _navigationTabParser.CountTab(html);

            if (index > count) return Result.Fail(new Retry($"Found {count} tabs but selected tab {index}"));

            var tab = _navigationTabParser.GetTab(html, index);
            if (_navigationTabParser.IsTabActive(tab)) return Result.Ok();
            Result result;
            result = await _mediator.Send(new ClickCommand(chromeBrowser, By.XPath(tab.XPath)));
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

            result = await _mediator.Send(new WaitCommand(chromeBrowser, tabActived));
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