﻿using FluentResults;
using HtmlAgilityPack;
using MainCore.Common.Commands;
using MainCore.Common.Errors;
using MainCore.Features.InstantUpgrade.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MediatR;
using OpenQA.Selenium;

namespace MainCore.Features.InstantUpgrade.Commands
{
    [RegisterAsTransient]
    public class InstantUpgradeCommand : IInstantUpgradeCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IInstantUpgradeParser _instantUpgradeParser;
        private readonly IMediator _mediator;

        public InstantUpgradeCommand(IInstantUpgradeParser instantUpgradeParser, IChromeManager chromeManager, IMediator mediator)
        {
            _instantUpgradeParser = instantUpgradeParser;
            _chromeManager = chromeManager;
            _mediator = mediator;
        }

        public async Task<Result> Execute(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;

            var completeNowButton = _instantUpgradeParser.GetCompleteButton(html);
            if (completeNowButton is null) return Result.Fail(Retry.ButtonNotFound("complete now"));

            Result result;

            result = await _mediator.Send(new ClickCommand(chromeBrowser, By.XPath(completeNowButton.XPath)));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            var confirmShown = new Func<IWebDriver, bool>(driver =>
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(driver.PageSource);
                var confirmButton = _instantUpgradeParser.GetConfirmButton(doc);
                return confirmButton is not null;
            });

            result = await _mediator.Send(new WaitCommand(chromeBrowser, confirmShown));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            html = chromeBrowser.Html;
            var confirmButton = _instantUpgradeParser.GetConfirmButton(html);
            if (confirmButton is null) return Result.Fail(Retry.ButtonNotFound("complete now"));

            result = await _mediator.Send(new ClickCommand(chromeBrowser, By.XPath(completeNowButton.XPath)));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            return Result.Ok();
        }
    }
}