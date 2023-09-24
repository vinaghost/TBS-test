using FluentResults;
using MainCore.Models;
using MainCore.Repositories;
using MainCore.Services;
using UpdateCore.Parsers;

namespace UpdateCore.Commands
{
    public class UpdateFieldCommand : IUpdateFieldCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IFieldParser _fieldParser;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IQueueBuildingRepository _queueBuildingRepository;

        public UpdateFieldCommand(IChromeManager chromeManager, IFieldParser fieldParser, IBuildingRepository buildingRepository, IQueueBuildingRepository queueBuildingRepository)
        {
            _chromeManager = chromeManager;
            _fieldParser = fieldParser;
            _buildingRepository = buildingRepository;
            _queueBuildingRepository = queueBuildingRepository;
        }

        public async Task<Result> Execute(IChromeBrowser chromeBrowser, int villageId)
        {
            var html = chromeBrowser.Html;

            var fields = _fieldParser.GetNodes(html);
            var buildings = fields.Select(x => new Building()
            {
                VillageId = villageId,
                Location = _fieldParser.GetId(x),
                Level = _fieldParser.GetLevel(x),
                Type = _fieldParser.GetBuildingType(x),
                IsUnderConstruction = _fieldParser.IsUnderConstruction(x),
            }).ToList();

            await _buildingRepository.Update(villageId, buildings);
            var isUnderConstructionList = buildings.Where(x => x.IsUnderConstruction).ToList();
            if (isUnderConstructionList.Count > 0)
            {
                await _queueBuildingRepository.Update(villageId, isUnderConstructionList);
            }
            return Result.Ok();
        }

        public async Task<Result> Execute(int accountId, int villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            return await Execute(chromeBrowser, villageId);
        }
    }
}