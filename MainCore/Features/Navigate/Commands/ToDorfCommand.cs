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
    public class ToDorfCommand : IToDorfCommand
    {
        private readonly INavigationBarParser _navigationBarParser;
        private readonly IChromeManager _chromeManager;

        public ToDorfCommand(INavigationBarParser navigationBarParser, IChromeManager chromeManager)

        {
            _navigationBarParser = navigationBarParser;
            _chromeManager = chromeManager;
        }

        public async Task<Result> Execute(AccountId accountId, int dorf)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;

            var button = GetButton(html, dorf);
            if (button is null) return Retry.ButtonNotFound($"dorf{dorf}");

            Result result;
            result = await chromeBrowser.Click(By.XPath(button.XPath));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = await chromeBrowser.WaitPageChanged($"dorf{dorf}");
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            return Result.Ok();
        }

        private HtmlNode GetButton(HtmlDocument doc, int dorf)
        {
            return dorf switch
            {
                1 => _navigationBarParser.GetResourceButton(doc),
                2 => _navigationBarParser.GetBuildingButton(doc),
                _ => null,
            };
        }
    }
}