using FluentResults;
using MainCore.Common.Commands;
using MainCore.Common.Errors;
using MainCore.Features.Farming.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using OpenQA.Selenium;

namespace MainCore.Features.Farming.Commands
{
    [RegisterAsTransient]
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
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }
    }
}