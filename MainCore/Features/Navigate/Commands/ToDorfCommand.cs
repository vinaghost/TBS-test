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
    public class ToDorfCommand : IToDorfCommand
    {
        private readonly INavigationBarParser _navigationBarParser;
        private readonly IChromeManager _chromeManager;

        private readonly IClickCommand _clickCommand;
        private readonly IWaitCommand _waitCommand;

        public ToDorfCommand(INavigationBarParser navigationBarParser, IChromeManager chromeManager, IClickCommand clickCommand, IWaitCommand waitCommand)

        {
            _navigationBarParser = navigationBarParser;
            _chromeManager = chromeManager;
            _clickCommand = clickCommand;
            _waitCommand = waitCommand;
        }

        public async Task<Result> Execute(IChromeBrowser chromeBrowser, int dorf)
        {
            var html = chromeBrowser.Html;

            var button = GetButton(html, dorf);
            if (button is null) return Retry.ButtonNotFound($"dorf{dorf}");

            Result result;
            result = await _clickCommand.Execute(chromeBrowser, By.XPath(button.XPath));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = await _waitCommand.Execute(chromeBrowser, WaitCommand.PageChanged($"dorf{dorf}"));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = await _waitCommand.Execute(chromeBrowser, WaitCommand.PageLoaded);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            return Result.Ok();
        }

        public async Task<Result> Execute(int accountId, int dorf)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            return await Execute(chromeBrowser, dorf);
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