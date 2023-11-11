using FluentResults;
using MainCore.Common.Errors;
using MainCore.Common.Models;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using OpenQA.Selenium;

namespace MainCore.Commands.Special
{
    [RegisterAsTransient]
    public class ConstructCommand : IConstructCommand
    {
        private readonly IChromeManager _chromeManager;

        public ConstructCommand(IChromeManager chromeManager)
        {
            _chromeManager = chromeManager;
        }

        public Result Execute(AccountId accountId, NormalBuildPlan plan)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var node = html.GetElementbyId($"contract_building{(int)plan.Type}");
            if (node is null) return Result.Fail(Retry.ButtonNotFound($"construct {plan.Type} [0]"));

            var button = node
                .Descendants("button")
                .FirstOrDefault(x => x.HasClass("new"));

            if (button is null) return Result.Fail(Retry.ButtonNotFound($"construct {plan.Type} [1]"));

            var result = chromeBrowser.Click(By.XPath(button.XPath));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = chromeBrowser.WaitPageChanged("dorf");
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = chromeBrowser.WaitPageLoaded();
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            return Result.Ok();
        }
    }
}