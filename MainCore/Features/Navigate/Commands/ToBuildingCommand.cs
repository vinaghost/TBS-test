using FluentResults;
using HtmlAgilityPack;
using MainCore.Common.Errors;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MediatR;
using OpenQA.Selenium;

namespace MainCore.Features.Navigate.Commands
{
    [RegisterAsTransient]
    public class ToBuildingCommand : IToBuildingCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IToDorfCommand _toDorfCommand;
        private readonly IMediator _mediator;

        public ToBuildingCommand(IChromeManager chromeManager, IToDorfCommand toDorfCommand, IMediator mediator)
        {
            _chromeManager = chromeManager;
            _toDorfCommand = toDorfCommand;
            _mediator = mediator;
        }

        public async Task<Result> Execute(int accountId, int location)
        {
            Result result;

            result = await _toDorfCommand.Execute(accountId, location < 19 ? 1 : 2);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

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
            result = await chromeBrowser.Click(By.XPath(node.XPath));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }
    }
}