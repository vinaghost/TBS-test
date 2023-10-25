using FluentResults;
using MainCore.Common.Errors;
using MainCore.Common.Repositories;
using MainCore.Entities;
using MainCore.Features.Navigate.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MediatR;
using OpenQA.Selenium;

namespace MainCore.Features.Navigate.Commands
{
    [RegisterAsTransient]
    public class SwitchVillageCommand : ISwitchVillageCommand
    {
        private readonly IVillageRepository _villageRepository;
        private readonly IChromeManager _chromeManager;
        private readonly IVillageItemParser _villageItemParser;
        private readonly IMediator _mediator;

        public SwitchVillageCommand(IVillageRepository villageRepository, IChromeManager chromeManager, IVillageItemParser villageItemParser, IMediator mediator)
        {
            _villageRepository = villageRepository;
            _chromeManager = chromeManager;
            _villageItemParser = villageItemParser;
            _mediator = mediator;
        }

        public async Task<Result> Execute(AccountId accountId, VillageId villageId)
        {
            var village = _villageRepository.GetById(villageId);
            if (village is null) return Skip.VillageNotFound;

            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var node = _villageItemParser.GetVillageNode(html, villageId);
            if (node is null) return Skip.VillageNotFound;

            if (_villageItemParser.IsActive(node)) return Result.Ok();

            Result result;
            result = await chromeBrowser.Click(By.XPath(node.XPath));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = await chromeBrowser.WaitPageChanged($"{villageId}");
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = await chromeBrowser.WaitPageLoaded();
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }
    }
}