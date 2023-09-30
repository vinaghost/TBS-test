using FluentResults;
using MainCore.Commands;
using MainCore.Errors;
using MainCore.Services;
using OpenQA.Selenium;
using UpdateCore.Parsers;

namespace FarmingCore.Commands
{
    public class SendFarmListCommand : ISendFarmListCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IClickCommand _clickCommand;
        private readonly IFarmListParser _farmListParser;

        public SendFarmListCommand(IChromeManager chromeManager, IClickCommand clickCommand, IFarmListParser farmListParser)
        {
            _chromeManager = chromeManager;
            _clickCommand = clickCommand;
            _farmListParser = farmListParser;
        }

        public async Task<Result> Execute(int accountId, int farmListId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var startButton = _farmListParser.GetStartButton(html, farmListId);
            if (startButton is null)
            {
                return Result.Fail(new Retry("Cannot found start button"));
            }

            var result = await _clickCommand.Execute(chromeBrowser, By.XPath(startButton.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}