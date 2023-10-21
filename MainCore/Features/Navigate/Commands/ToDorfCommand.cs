using FluentResults;
using HtmlAgilityPack;
using MainCore.Common.Commands;
using MainCore.Common.Errors;
using MainCore.Features.Navigate.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MediatR;
using OpenQA.Selenium;

namespace MainCore.Features.Navigate.Commands
{
    [RegisterAsTransient]
    public class ToDorfCommand : IToDorfCommand
    {
        private readonly INavigationBarParser _navigationBarParser;
        private readonly IChromeManager _chromeManager;
        private readonly IMediator _mediator;

        public ToDorfCommand(INavigationBarParser navigationBarParser, IChromeManager chromeManager, IMediator mediator)

        {
            _navigationBarParser = navigationBarParser;
            _chromeManager = chromeManager;
            _mediator = mediator;
        }

        public async Task<Result> Execute(IChromeBrowser chromeBrowser, int dorf)
        {
            var html = chromeBrowser.Html;

            var button = GetButton(html, dorf);
            if (button is null) return Retry.ButtonNotFound($"dorf{dorf}");

            Result result;
            result = await _mediator.Send(new ClickCommand(chromeBrowser, By.XPath(button.XPath)));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = await _mediator.Send(new WaitCommand(chromeBrowser, WaitCommand.PageChanged($"dorf{dorf}")));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = await _mediator.Send(new WaitCommand(chromeBrowser, WaitCommand.PageLoaded));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            return Result.Ok();
        }

        public async Task<Result> Execute(int accountId, int dorf)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            return await Execute(chromeBrowser, dorf);
        }

        private HtmlNode GetButton(HtmlDocument doc, int dorf)
        {
            return dorf switch
            {
                1 => _navigationBarParser.GetResourceButton(doc),
                2 => _navigationBarParser.GetBuildingButton(doc),
                _ => null,
            };
        }
    }
}