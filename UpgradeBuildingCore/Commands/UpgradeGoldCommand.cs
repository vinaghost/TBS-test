using FluentResults;
using MainCore.Commands;
using MainCore.Errors;
using MainCore.Models.Plans;
using MainCore.Services;
using OpenQA.Selenium;

namespace UpgradeBuildingCore.Commands
{
    public class UpgradeGoldCommand : IUpgradeGoldCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IClickCommand _clickCommand;

        public UpgradeGoldCommand(IChromeManager chromeManager, IClickCommand clickCommand)
        {
            _chromeManager = chromeManager;
            _clickCommand = clickCommand;
        }

        public async Task<Result> Execute(int accountId, NormalBuildPlan plan)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var node = html.DocumentNode
                .Descendants("div")
                .FirstOrDefault(x => x.HasClass("upgradeButtonsContainer"));
            if (node is null) return Result.Fail(Retry.ButtonNotFound($"upgrade {plan.Type} [0]"));

            var button = node
                .Descendants("button")
            .FirstOrDefault(x => x.HasClass("build"));

            if (button is null) return Result.Fail(Retry.ButtonNotFound($"upgrade {plan.Type} [1]"));

            var result = await _clickCommand.Execute(chromeBrowser, By.XPath(button.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}