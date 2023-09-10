using FluentResults;
using MainCore.Errors;
using MainCore.Models;
using MainCore.Repositories;
using MainCore.Services;
using UpdateCore.Parsers;

namespace UpdateCore.Commands
{
    public class UpdateVillageListCommand : IUpdateVillageListCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IVillageListParser _villageListParser;
        private readonly IVillageRepository _villageRepository;

        public UpdateVillageListCommand(IChromeManager chromeManager, IVillageListParser villageListParser, IVillageRepository villageRepository)
        {
            _chromeManager = chromeManager;
            _villageListParser = villageListParser;
            _villageRepository = villageRepository;
        }

        public async Task<Result> Execute(int accountId)
        {
            return await Task.Run(() => ExecuteSync(accountId));
        }

        private Result ExecuteSync(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var listNode = _villageListParser.GetVillages(html);
            if (listNode.Count == 0) return Result.Fail(new Retry("Villages list is empty"));
            var foundVills = new List<Village>();
            foreach (var node in listNode)
            {
                var id = _villageListParser.GetId(node);
                var name = _villageListParser.GetName(node);
                var x = _villageListParser.GetX(node);
                var y = _villageListParser.GetY(node);
                foundVills.Add(new()
                {
                    AccountId = accountId,
                    Id = id,
                    Name = name,
                    X = x,
                    Y = y,
                    IsLoaded = false,
                });
            }

            _villageRepository.Update(accountId, foundVills);
            return Result.Ok();
        }
    }
}