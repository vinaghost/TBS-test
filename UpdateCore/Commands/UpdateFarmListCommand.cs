using FluentResults;
using MainCore.Models;
using MainCore.Repositories;
using MainCore.Services;
using UpdateCore.Parsers;

namespace UpdateCore.Commands
{
    public class UpdateFarmListCommand : IUpdateFarmListCommand
    {
        private readonly IFarmListRepository _farmListRepository;
        private readonly IChromeManager _chromeManager;
        private readonly IFarmListParser _farmListParser;

        public UpdateFarmListCommand(IFarmListRepository farmListRepository, IChromeManager chromeManager, IFarmListParser farmListParser)
        {
            _farmListRepository = farmListRepository;
            _chromeManager = chromeManager;
            _farmListParser = farmListParser;
        }

        public async Task<Result> Execute(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var nodes = _farmListParser.GetFarmNodes(html);
            var foundFarmLists = new List<FarmList>();

            foreach (var node in nodes)
            {
                var id = _farmListParser.GetId(node);
                var name = _farmListParser.GetName(node);
                foundFarmLists.Add(new()
                {
                    AccountId = accountId,
                    Id = id,
                    Name = name,
                });
            }

            await _farmListRepository.Update(accountId, foundFarmLists);
            return Result.Ok();
        }
    }
}