using FluentResults;
using HtmlAgilityPack;
using MainCore.Commands;
using MainCore.Errors;
using MainCore.Services;
using NavigateCore.Parsers;
using OpenQA.Selenium;

namespace NavigateCore.Commands
{
    public class ToHeroInventoryCommand : IToHeroInventoryCommand
    {
        private readonly IHeroParser _heroParser;
        private readonly IChromeManager _chromeManager;
        private readonly IClickCommand _clickCommand;
        private readonly ISwitchTabCommand _switchTabCommand;
        private readonly IWaitCommand _waitCommand;

        public ToHeroInventoryCommand(IHeroParser heroParser, IChromeManager chromeManager, IClickCommand clickCommand, ISwitchTabCommand switchTabCommand, IWaitCommand waitCommand)
        {
            _heroParser = heroParser;
            _chromeManager = chromeManager;
            _clickCommand = clickCommand;
            _switchTabCommand = switchTabCommand;
            _waitCommand = waitCommand;
        }

        public async Task<Result> Execute(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var avatar = _heroParser.GetHeroAvatar(html);
            if (avatar is null) return Result.Fail(Retry.ButtonNotFound("avatar hero"));

            Result result;
            result = await _clickCommand.Execute(chromeBrowser, By.XPath(avatar.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = await _waitCommand.Execute(chromeBrowser, WaitCommand.PageChanged("hero"));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await _waitCommand.Execute(chromeBrowser, WaitCommand.PageLoaded);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage())); result = await _waitCommand.Execute(chromeBrowser, WaitCommand.PageLoaded);
            result = await _waitCommand.Execute(chromeBrowser, driver =>
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(driver.PageSource);
                var tab = _heroParser.GetHeroTab(doc, 1); // data-index not index in list
                if (tab is null) return false;
                return _heroParser.IsCurrentTab(tab);
            });
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = await _switchTabCommand.Execute(chromeBrowser, 0);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}