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
    public class SendAllFarmListCommand : ISendAllFarmListCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IFarmListParser _farmListParser;

        public SendAllFarmListCommand(IChromeManager chromeManager, IFarmListParser farmListParser)
        {
            _chromeManager = chromeManager;
            _farmListParser = farmListParser;
        }

        public Result Execute(AccountId accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;

            var startAllButton = _farmListParser.GetStartAllButton(html);
            if (startAllButton is null)
            {
                return Result.Fail(new Retry("Cannot found start all button"));
            }

            var result = chromeBrowser.Click(By.XPath(startAllButton.XPath));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }
    }
}