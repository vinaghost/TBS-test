using FluentResults;
using HtmlAgilityPack;
using MainCore.Common.Commands;
using MainCore.Common.Errors;
using MainCore.Common.Models;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MediatR;
using OpenQA.Selenium;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    [RegisterAsTransient]
    public class UpgradeAdsCommand : IUpgradeAdsCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IMediator _mediator;

        public UpgradeAdsCommand(IChromeManager chromeManager, IMediator mediator)
        {
            _chromeManager = chromeManager;
            _mediator = mediator;
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

            var result = await _mediator.Send(new ClickCommand(chromeBrowser, By.XPath(button.XPath)));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            var driver = chromeBrowser.Driver;
            var current = driver.CurrentWindowHandle;
            while (driver.WindowHandles.Count > 1)
            {
                var others = driver.WindowHandles.FirstOrDefault(x => !x.Equals(current));
                driver.SwitchTo().Window(others);
                driver.Close();
                driver.SwitchTo().Window(current);
            }

            var videoFeatureShown = new Func<IWebDriver, bool>(driver =>
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(driver.PageSource);
                return doc.GetElementbyId("videoFeature") is not null;
            });

            result = await _mediator.Send(new WaitCommand(chromeBrowser, videoFeatureShown));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            await Task.Delay(Random.Shared.Next(20000, 25000));

            html = chromeBrowser.Html;
            node = html.GetElementbyId("videoFeature");
            if (node is null) return Result.Fail(Retry.ButtonNotFound($"play ads"));

            result = await _mediator.Send(new ClickCommand(chromeBrowser, By.XPath(node.XPath)));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            driver.SwitchTo().DefaultContent();

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

                result = await _mediator.Send(new ClickCommand(chromeBrowser, By.XPath(node.XPath)));
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

                driver.SwitchTo().DefaultContent();
            }
            while (true);

            result = await _mediator.Send(new WaitCommand(chromeBrowser, WaitCommand.PageChanged("dorf")));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = await _mediator.Send(new WaitCommand(chromeBrowser, WaitCommand.PageLoaded));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            return Result.Ok();
        }
    }
}