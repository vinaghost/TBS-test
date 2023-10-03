using FluentResults;
using MainCore.Common.Repositories;
using MainCore.Entities;
using MainCore.Features.Update.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;

namespace MainCore.Features.Update.Commands
{
    [RegisterAsTransient]
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