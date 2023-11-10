using FluentResults;
using MainCore.Common.Errors;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.Parsers;
using OpenQA.Selenium;

namespace MainCore.Features.Navigate.Commands
{
    [RegisterAsTransient]
    public class ToBuildingCommand : IToBuildingCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IBuildingParser _buildingParser;

        public ToBuildingCommand(IChromeManager chromeManager, IBuildingParser buildingParser)
        {
            _chromeManager = chromeManager;
            _buildingParser = buildingParser;
        }

        public Result Execute(AccountId accountId, int location)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var node = _buildingParser.GetBuilding(html, location);
            if (node is null) return Result.Fail(Retry.NotFound($"{location}", "nodeBuilding"));

            Result result;
            result = chromeBrowser.Click(By.XPath(node.XPath));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = chromeBrowser.WaitPageLoaded();
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }
    }
}