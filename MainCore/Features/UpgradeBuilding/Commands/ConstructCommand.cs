using FluentResults;
using MainCore.Common.Commands;
using MainCore.Common.Errors;
using MainCore.Common.Models;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using OpenQA.Selenium;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    [RegisterAsTransient]
    public class ConstructCommand : IConstructCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IClickCommand _clickCommand;
        private readonly IWaitCommand _waitCommand;

        public ConstructCommand(IChromeManager chromeManager, IClickCommand clickCommand, IWaitCommand waitCommand)
        {
            _chromeManager = chromeManager;
            _clickCommand = clickCommand;
            _waitCommand = waitCommand;
        }

        public async Task<Result> Execute(int accountId, NormalBuildPlan plan)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var node = html.GetElementbyId($"contract_building{(int)plan.Type}");
            if (node is null) return Result.Fail(Retry.ButtonNotFound($"construct {plan.Type} [0]"));

            var button = node
                .Descendants("button")
                .FirstOrDefault(x => x.HasClass("new"));

            if (button is null) return Result.Fail(Retry.ButtonNotFound($"construct {plan.Type} [1]"));

            var result = await _clickCommand.Execute(chromeBrowser, By.XPath(button.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = await _waitCommand.Execute(chromeBrowser, WaitCommand.PageChanged("dorf"));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await _waitCommand.Execute(chromeBrowser, WaitCommand.PageLoaded);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}