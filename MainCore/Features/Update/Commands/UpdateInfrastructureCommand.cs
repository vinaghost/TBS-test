using FluentResults;
using MainCore.Common.Repositories;
using MainCore.Entities;
using MainCore.Features.Update.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;

namespace MainCore.Features.Update.Commands
{
    [RegisterAsTransient]
    public class UpdateInfrastructureCommand : IUpdateInfrastructureCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IInfrastructureParser _infrastructureParser;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IQueueBuildingRepository _queueBuildingRepository;

        public UpdateInfrastructureCommand(IChromeManager chromeManager, IBuildingRepository buildingRepository, IInfrastructureParser infrastructureParser, IQueueBuildingRepository queueBuildingRepository)
        {
            _chromeManager = chromeManager;
            _buildingRepository = buildingRepository;
            _infrastructureParser = infrastructureParser;
            _queueBuildingRepository = queueBuildingRepository;
        }

        public async Task<Result> Execute(IChromeBrowser chromeBrowser, int villageId)
        {
            var html = chromeBrowser.Html;

            var fields = _infrastructureParser.GetNodes(html);
            var buildings = fields
                .Select(x => new Building()
                {
                    VillageId = villageId,
                    Location = _infrastructureParser.GetId(x),
                    Level = _infrastructureParser.GetLevel(x),
                    Type = _infrastructureParser.GetBuildingType(x),
                    IsUnderConstruction = _infrastructureParser.IsUnderConstruction(x),
                })
                .ToList();

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