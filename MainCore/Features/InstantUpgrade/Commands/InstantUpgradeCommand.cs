using FluentResults;
using HtmlAgilityPack;
using MainCore.Common.Commands;
using MainCore.Common.Errors;
using MainCore.Features.InstantUpgrade.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using OpenQA.Selenium;

namespace MainCore.Features.InstantUpgrade.Commands
{
    [RegisterAsTransient]
    public class InstantUpgradeCommand : IInstantUpgradeCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IInstantUpgradeParser _instantUpgradeParser;
        private readonly IClickCommand _clickCommand;
        private readonly IWaitCommand _waitCommand;

        public InstantUpgradeCommand(IInstantUpgradeParser instantUpgradeParser, IChromeManager chromeManager, IClickCommand clickCommand, IWaitCommand waitCommand)
        {
            _instantUpgradeParser = instantUpgradeParser;
            _chromeManager = chromeManager;
            _clickCommand = clickCommand;
            _waitCommand = waitCommand;
        }

        public async Task<Result> Execute(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;

            var completeNowButton = _instantUpgradeParser.GetCompleteButton(html);
            if (completeNowButton is null) return Result.Fail(Retry.ButtonNotFound("complete now"));

            Result result;

            result = await _clickCommand.Execute(chromeBrowser, By.XPath(completeNowButton.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = await _waitCommand.Execute(chromeBrowser, driver =>
            {
                var html = new HtmlDocument();
                var confirmButton = _instantUpgradeParser.GetConfirmButton(html);
                return confirmButton is not null;
            });
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            html = chromeBrowser.Html;
            var confirmButton = _instantUpgradeParser.GetConfirmButton(html);
            if (confirmButton is null) return Result.Fail(Retry.ButtonNotFound("complete now"));

            result = await _clickCommand.Execute(chromeBrowser, By.XPath(completeNowButton.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}