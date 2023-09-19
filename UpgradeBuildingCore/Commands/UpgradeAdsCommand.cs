using FluentResults;
using MainCore.Commands;
using MainCore.Errors;
using MainCore.Models.Plans;
using MainCore.Services;
using OpenQA.Selenium;

namespace UpgradeBuildingCore.Commands
{
    public class UpgradeAdsCommand : IUpgradeAdsCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IClickCommand _clickCommand;

        public UpgradeAdsCommand(IChromeManager chromeManager, IClickCommand clickCommand)
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
            if (node is null) return Result.Fail(Retry.ButtonNotFound($"upgrade {plan.Type} with ads [0]"));

            var button = node
                .Descendants("button")
                .FirstOrDefault(x => x.HasClass("videoFeatureButton") && x.HasClass("green"));

            if (button is null) return Result.Fail(Retry.ButtonNotFound($"upgrade {plan.Type} with ads [1]"));

            var result = await _clickCommand.Execute(chromeBrowser, By.XPath(button.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            var driver = chromeBrowser.Driver;
            var current = driver.CurrentWindowHandle;
            while (driver.WindowHandles.Count > 1)
            {
                var others = driver.WindowHandles.FirstOrDefault(x => !x.Equals(current));
                driver.SwitchTo().Window(others);
                driver.Close();
                driver.SwitchTo().Window(current);
            }
            node = html.GetElementbyId("videoFeature");
            if (node is null) return Result.Fail(Retry.ButtonNotFound($"play ads"));

            result = await _clickCommand.Execute(chromeBrowser, By.XPath(node.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            driver.SwitchTo().DefaultContent();

            await Task.Delay(Random.Shared.Next(1300, 2000));
            // close if bot click on playing ads
            // chrome will open new tab & pause ads
            do
            {
                var handles = driver.WindowHandles;
                if (handles.Count == 1) break;

                current = driver.CurrentWindowHandle;
                var other = driver.WindowHandles.FirstOrDefault(x => !x.Equals(current));
                driver.SwitchTo().Window(other);
                driver.Close();
                driver.SwitchTo().Window(current);

                result = await _clickCommand.Execute(chromeBrowser, By.XPath(node.XPath));
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

                driver.SwitchTo().DefaultContent();
            }
            while (true);

            return Result.Ok();
        }
    }
}