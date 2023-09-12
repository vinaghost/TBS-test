using FluentResults;
using MainCore.Commands;
using MainCore.Errors;
using MainCore.Repositories;
using MainCore.Services;
using NavigateCore.Parsers;
using OpenQA.Selenium;

namespace NavigateCore.Commands
{
    public class SwitchVillageCommand : ISwitchVillageCommand
    {
        private readonly IVillageRepository _villageRepository;
        private readonly IChromeManager _chromeManager;
        private readonly IVillageItemParser _villageItemParser;
        private readonly IClickButtonCommand _clickButtonCommand;
        private readonly IWaitCommand _waitCommand;

        public SwitchVillageCommand(IVillageRepository villageRepository, IChromeManager chromeManager, IVillageItemParser villageItemParser, IClickButtonCommand clickButtonCommand, IWaitCommand waitCommand)
        {
            _villageRepository = villageRepository;
            _chromeManager = chromeManager;
            _villageItemParser = villageItemParser;
            _clickButtonCommand = clickButtonCommand;
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
            result = await _clickButtonCommand.Execute(chromeBrowser, By.XPath(node.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = await _waitCommand.Execute(accountId, WaitCommand.PageChanged($"{villageId}"));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await _waitCommand.Execute(accountId, WaitCommand.PageLoaded);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}