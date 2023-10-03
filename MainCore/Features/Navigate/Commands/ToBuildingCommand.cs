using FluentResults;
using HtmlAgilityPack;
using MainCore.Common.Commands;
using MainCore.Common.Errors;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using OpenQA.Selenium;

namespace MainCore.Features.Navigate.Commands
{
    [RegisterAsTransient]
    public class ToBuildingCommand : IToBuildingCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IToDorfCommand _toDorfCommand;
        private readonly IClickCommand _clickCommand;

        public ToBuildingCommand(IChromeManager chromeManager, IClickCommand clickCommand, IToDorfCommand toDorfCommand)
        {
            _chromeManager = chromeManager;
            _clickCommand = clickCommand;
            _toDorfCommand = toDorfCommand;
        }

        public async Task<Result> Execute(int accountId, int location)
        {
            Result result;

            result = await _toDorfCommand.Execute(accountId, location < 19 ? 1 : 2);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            HtmlNode node;

            if (location < 19)
            {
                node = html.DocumentNode
                   .Descendants("a")
                   .FirstOrDefault(x => x.HasClass($"buildingSlot{location}"));
            }
            else
            {
                var tmpLocation = location - 18;
                node = html.DocumentNode
                    .SelectSingleNode($"//*[@id='villageContent']/div[{tmpLocation}]");
            }

            if (node is null) return Result.Fail(Retry.NotFound($"{location}", "nodeBuilding"));
            result = await _clickCommand.Execute(chromeBrowser, By.XPath(node.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}