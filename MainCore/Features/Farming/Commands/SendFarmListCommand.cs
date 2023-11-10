using FluentResults;
using MainCore.Common.Errors;
using MainCore.Entities;
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
        private readonly IFarmListParser _farmListParser;

        public SendFarmListCommand(IChromeManager chromeManager, IFarmListParser farmListParser)
        {
            _chromeManager = chromeManager;
            _farmListParser = farmListParser;
        }

        public Result Execute(AccountId accountId, FarmId farmListId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var startButton = _farmListParser.GetStartButton(html, farmListId);
            if (startButton is null)
            {
                return Result.Fail(new Retry("Cannot found start button"));
            }

            var result = chromeBrowser.Click(By.XPath(startButton.XPath));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }
    }
}