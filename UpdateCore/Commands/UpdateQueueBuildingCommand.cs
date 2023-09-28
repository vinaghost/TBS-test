using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Models;
using MainCore.Repositories;
using MainCore.Services;
using UpdateCore.Parsers;

namespace UpdateCore.Commands
{
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

            var nodes = _queueBuildingParser.GetNodes(html);

            var queueBuildings = new List<QueueBuilding>();
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                var strType = _queueBuildingParser.GetBuildingType(node);
                var resultParse = Enum.TryParse(strType, false, out BuildingEnums type);
                if (!resultParse) return Result.Fail(Stop.EnglishRequired(strType));
                var level = _queueBuildingParser.GetLevel(node);
                var duration = _queueBuildingParser.GetDuration(node);

                queueBuildings.Add(new()
                {
                    Position = i,
                    VillageId = villageId,
                    Type = type,
                    Level = level,
                    CompleteTime = DateTime.Now.Add(duration),
                    Location = -1,
                });
            }

            for (int i = nodes.Count; i < 4; i++) // we will save 3 slot for each village, Roman can build 3 building in one time
            {
                queueBuildings.Add(new()
                {
                    Position = i,
                    VillageId = villageId,
                    Type = BuildingEnums.Site,
                    Level = -1,
                    CompleteTime = DateTime.MaxValue,
                    Location = -1,
                });
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