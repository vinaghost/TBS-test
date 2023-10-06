using FluentResults;
using MainCore.Common.Enums;
using MainCore.Common.Errors;
using MainCore.Common.Repositories;
using MainCore.Entities;
using MainCore.Features.Update.DTO;
using MainCore.Features.Update.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;

namespace MainCore.Features.Update.Commands
{
    [RegisterAsTransient]
    public class UpdateQueueBuildingCommand : IUpdateQueueBuildingCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IQueueBuildingParser _queueBuildingParser;
        private readonly IQueueBuildingRepository _queueBuildingRepository;

        public UpdateQueueBuildingCommand(IChromeManager chromeManager, IQueueBuildingParser queueBuildingParser, IQueueBuildingRepository queueBuildingRepository)
        {
            _chromeManager = chromeManager;
            _queueBuildingParser = queueBuildingParser;
            _queueBuildingRepository = queueBuildingRepository;
        }

        public async Task<Result> Execute(IChromeBrowser chromeBrowser, int villageId)
        {
            var html = chromeBrowser.Html;

            var dtos = _queueBuildingParser.Get(html);
            var mapper = new QueueBuildingMapper();
            var queueBuildings = new List<QueueBuilding>();

            foreach (var dto in dtos)
            {
                var strType = dto.Type;
                var resultParse = Enum.TryParse(strType, false, out BuildingEnums type);
                if (!resultParse) return Result.Fail(Stop.EnglishRequired(strType));

                var buildingQueue = mapper.Map(villageId, type, dto);
                queueBuildings.Add(buildingQueue);
            }

            await _queueBuildingRepository.Update(villageId, queueBuildings);
            return Result.Ok();
        }

        public async Task<Result> Execute(int accountId, int villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            return await Execute(chromeBrowser, villageId);
        }
    }
}