using FluentResults;
using MainCore.Common.Errors;
using MainCore.Common.Models;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MediatR;
using OpenQA.Selenium;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    [RegisterAsTransient]
    public class UpgradeGoldCommand : IUpgradeGoldCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IMediator _mediator;

        public UpgradeGoldCommand(IChromeManager chromeManager, IMediator mediator)
        {
            _chromeManager = chromeManager;
            _mediator = mediator;
        }

        public async Task<Result> Execute(AccountId accountId, NormalBuildPlan plan)
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

            var result = await chromeBrowser.Click(By.XPath(button.XPath));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }
    }
}