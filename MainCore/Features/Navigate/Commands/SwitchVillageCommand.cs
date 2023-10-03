using FluentResults;
using MainCore.Common.Commands;
using MainCore.Common.Errors;
using MainCore.Common.Repositories;
using MainCore.Features.Navigate.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using OpenQA.Selenium;

namespace MainCore.Features.Navigate.Commands
{
    [RegisterAsTransient]
    public class SwitchVillageCommand : ISwitchVillageCommand
    {
        private readonly IVillageRepository _villageRepository;
        private readonly IChromeManager _chromeManager;
        private readonly IVillageItemParser _villageItemParser;
        private readonly IClickCommand _clickCommand;
        private readonly IWaitCommand _waitCommand;

        public SwitchVillageCommand(IVillageRepository villageRepository, IChromeManager chromeManager, IVillageItemParser villageItemParser, IClickCommand clickCommand, IWaitCommand waitCommand)
        {
            _villageRepository = villageRepository;
            _chromeManager = chromeManager;
            _villageItemParser = villageItemParser;
            _clickCommand = clickCommand;
            _waitCommand = waitCommand;
        }

        public async Task<Result> Execute(int accountId, int villageId)
        {
            var village = await _villageRepository.Get(villageId);
            if (village is null) return Skip.VillageNotFound;

            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var node = _villageItemParser.GetVillageNode(html, villageId);
            if (node is null) return Skip.VillageNotFound;

            if (_villageItemParser.IsActive(node)) return Result.Ok();

            Result result;
            result = await _clickCommand.Execute(chromeBrowser, By.XPath(node.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = await _waitCommand.Execute(chromeBrowser, WaitCommand.PageChanged($"{villageId}"));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await _waitCommand.Execute(chromeBrowser, WaitCommand.PageLoaded);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}