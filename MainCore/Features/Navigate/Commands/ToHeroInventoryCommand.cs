using FluentResults;
using HtmlAgilityPack;
using MainCore.Common.Errors;
using MainCore.Entities;
using MainCore.Features.Navigate.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MediatR;
using OpenQA.Selenium;

namespace MainCore.Features.Navigate.Commands
{
    [RegisterAsTransient]
    public class ToHeroInventoryCommand : IToHeroInventoryCommand
    {
        private readonly IHeroParser _heroParser;
        private readonly IChromeManager _chromeManager;
        private readonly ISwitchTabCommand _switchTabCommand;
        private readonly IMediator _mediator;

        public ToHeroInventoryCommand(IHeroParser heroParser, IChromeManager chromeManager, ISwitchTabCommand switchTabCommand, IMediator mediator)
        {
            _heroParser = heroParser;
            _chromeManager = chromeManager;
            _switchTabCommand = switchTabCommand;
            _mediator = mediator;
        }

        public async Task<Result> Execute(AccountId accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var avatar = _heroParser.GetHeroAvatar(html);
            if (avatar is null) return Result.Fail(Retry.ButtonNotFound("avatar hero"));

            Result result;
            result = await chromeBrowser.Click(By.XPath(avatar.XPath));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = await chromeBrowser.WaitPageChanged("hero");
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = await chromeBrowser.WaitPageLoaded();
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            bool tabActived(IWebDriver driver)
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(driver.PageSource);
                var tab = _heroParser.GetHeroTab(doc, 1); // data-index not index in list
                if (tab is null) return false;
                return _heroParser.IsCurrentTab(tab);
            };

            result = await chromeBrowser.Wait(tabActived);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = await _switchTabCommand.Execute(chromeBrowser, 0);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            return Result.Ok();
        }
    }
}