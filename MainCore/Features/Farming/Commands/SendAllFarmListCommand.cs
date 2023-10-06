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
    public class SendAllFarmListCommand : ISendAllFarmListCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IClickCommand _clickCommand;
        private readonly IFarmListParser _farmListParser;

        public SendAllFarmListCommand(IClickCommand clickCommand, IChromeManager chromeManager, IFarmListParser farmListParser)
        {
            _clickCommand = clickCommand;
            _chromeManager = chromeManager;
            _farmListParser = farmListParser;
        }

        public async Task<Result> Execute(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;

            var startAllButton = _farmListParser.GetStartAllButton(html);
            if (startAllButton is null)
            {
                return Result.Fail(new Retry("Cannot found start all button"));
            }

            var result = await _clickCommand.Execute(chromeBrowser, By.XPath(startAllButton.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}