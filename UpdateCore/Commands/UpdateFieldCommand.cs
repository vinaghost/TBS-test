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

        public UpdateFieldCommand(IChromeManager chromeManager, IFieldParser fieldParser, IBuildingRepository buildingRepository)
        {
            _chromeManager = chromeManager;
            _fieldParser = fieldParser;
            _buildingRepository = buildingRepository;
        }

        public async Task<Result> Execute(int accountId, int villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;

            var fields = _fieldParser.GetNodes(html);
            var buildings = fields.Select(x => new Building()
            {
                VillageId = villageId,
                Id = _fieldParser.GetId(x),
                Level = _fieldParser.GetLevel(x),
                Type = _fieldParser.GetBuildingType(x),
                IsUnderConstruction = _fieldParser.IsUnderConstruction(x),
            }).ToList();

            await _buildingRepository.Update(villageId, buildings);
            return Result.Ok();
        }
    }
}