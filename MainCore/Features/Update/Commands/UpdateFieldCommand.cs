using FluentResults;
using MainCore.Common.Repositories;
using MainCore.Entities;
using MainCore.Features.Update.DTO;
using MainCore.Features.Update.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;

namespace MainCore.Features.Update.Commands
{
    [RegisterAsTransient]
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

            var dtos = _fieldParser.Get(html);

            var mapper = new BuildingMapper();
            var buildings = new List<Building>();
            foreach (var dto in dtos)
            {
                var field = mapper.Map(villageId, dto);
                buildings.Add(field);
            }

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