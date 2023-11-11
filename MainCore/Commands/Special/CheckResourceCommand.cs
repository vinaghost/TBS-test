using FluentResults;
using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Common.Errors;
using MainCore.Common.Models;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.Repositories;

namespace MainCore.Commands.Special
{
    [RegisterAsTransient]
    public class CheckResourceCommand : ICheckResourceCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IStorageRepository _storageRepository;

        public long[] Value { get; private set; }

        public CheckResourceCommand(IChromeManager chromeManager, IBuildingRepository buildingRepository, IStorageRepository storageRepository)
        {
            _chromeManager = chromeManager;
            _buildingRepository = buildingRepository;
            _storageRepository = storageRepository;
        }

        public async Task<Result> Execute(AccountId accountId, VillageId villageId, NormalBuildPlan plan)
        {
            Result result;
            result = GetBuildingResource(accountId, villageId, plan);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = await Task.Run(() => _storageRepository.IsEnoughResource(villageId, Value));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            return Result.Ok();
        }

        private Result GetBuildingResource(AccountId accountId, VillageId villageId, NormalBuildPlan plan)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;

            var building = _buildingRepository.GetBuilding(villageId, plan.Location);
            HtmlNode node;
            if (building.Type == BuildingEnums.Site)
            {
                node = html.GetElementbyId($"contract_building{(int)plan.Type}");
            }
            else
            {
                node = html.GetElementbyId("contract");
            }

            if (node is null) return Result.Fail(new Retry("Cannot read rescource requirement [0]"));
            node = node.Descendants("div").FirstOrDefault(x => x.HasClass("resourceWrapper"));
            if (node is null) return Result.Fail(new Retry("Cannot read rescource requirement [1]"));
            var nodes = node.ChildNodes.Where(x => x.HasClass("resource") || x.HasClass("resources")).ToList();
            if (nodes.Count != 5) return Result.Fail(new Retry("Cannot read rescource requirement [2]"));
            var resourceBuilding = new long[5];
            for (var i = 0; i < 5; i++)
            {
                var strResult = new string(nodes[i].InnerText.Where(c => char.IsDigit(c)).ToArray());
                if (string.IsNullOrEmpty(strResult)) resourceBuilding[i] = 0;
                else resourceBuilding[i] = long.Parse(strResult);
            }

            Value = resourceBuilding;
            return Result.Ok();
        }
    }
}