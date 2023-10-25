using FluentResults;
using MainCore.Common.Errors;
using MainCore.Entities;
using MainCore.Features.Farming.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MediatR;
using OpenQA.Selenium;

namespace MainCore.Features.Farming.Commands
{
    [RegisterAsTransient]
    public class SendFarmListCommand : ISendFarmListCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IFarmListParser _farmListParser;
        private readonly IMediator _mediator;

        public SendFarmListCommand(IChromeManager chromeManager, IFarmListParser farmListParser, IMediator mediator)
        {
            _chromeManager = chromeManager;
            _farmListParser = farmListParser;
            _mediator = mediator;
        }

        public async Task<Result> Execute(AccountId accountId, FarmListId farmListId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var startButton = _farmListParser.GetStartButton(html, farmListId);
            if (startButton is null)
            {
                return Result.Fail(new Retry("Cannot found start button"));
            }

            var result = await chromeBrowser.Click(By.XPath(startButton.XPath));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }
    }
}