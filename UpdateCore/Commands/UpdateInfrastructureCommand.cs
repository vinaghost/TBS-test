using FluentResults;
using MainCore.Models;
using MainCore.Repositories;
using MainCore.Services;
using UpdateCore.Parsers;

namespace UpdateCore.Commands
{
    public class UpdateInfrastructureCommand : IUpdateInfrastructureCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IInfrastructureParser _infrastructureParser;
        private readonly IBuildingRepository _buildingRepository;

        public UpdateInfrastructureCommand(IChromeManager chromeManager, IBuildingRepository buildingRepository, IInfrastructureParser infrastructureParser)
        {
            _chromeManager = chromeManager;
            _buildingRepository = buildingRepository;
            _infrastructureParser = infrastructureParser;
        }

        public async Task<Result> Execute(int accountId, int villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;

            var fields = _infrastructureParser.GetNodes(html);
            var buildings = fields
                .Select(x => new Building()
                {
                    VillageId = villageId,
                    Id = _infrastructureParser.GetId(x),
                    Level = _infrastructureParser.GetLevel(x),
                    Type = _infrastructureParser.GetBuildingType(x),
                    IsUnderConstruction = _infrastructureParser.IsUnderConstruction(x),
                })
                .ToList();

            await _buildingRepository.Update(villageId, buildings);
            return Result.Ok();
        }
    }
}