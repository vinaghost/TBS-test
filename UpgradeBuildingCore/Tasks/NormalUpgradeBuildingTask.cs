using FluentResults;
using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Errors.Storage;
using MainCore.Models.Plans;
using MainCore.Repositories;
using MainCore.Services;
using MainCore.Tasks;
using System.Text.Json;
using UpgradeBuildingCore.Commands;

namespace UpgradeBuildingCore.Tasks
{
    public class NormalUpgradeBuildingTask : VillageTask
    {
        private readonly IChooseBuildingJobCommand _chooseBuildingJobCommand;
        private readonly IExtractResourceFieldCommand _extractResourceFieldCommand;
        private readonly IGoToBuildingPageCommand _goToBuildingPageCommand;
        private readonly IUpgradeCommand _upgradeCommand;
        private readonly IConstructCommand _constructCommand;
        private readonly IBuildingRepository _buildingRepository;
        private readonly ICheckResourceCommand _checkResourceCommand;
        private readonly IAddCroplandCommand _addCroplandCommand;
        private readonly IChromeManager _chromeManager;

        public NormalUpgradeBuildingTask(IChooseBuildingJobCommand chooseBuildingJobCommand, IExtractResourceFieldCommand extractResourceFieldCommand, IGoToBuildingPageCommand goToBuildingPageCommand, IUpgradeCommand upgradeCommand, IBuildingRepository buildingRepository, IConstructCommand constructCommand, IChromeManager chromeManager, ICheckResourceCommand checkResourceCommand, IAddCroplandCommand addCroplandCommand)
        {
            _chooseBuildingJobCommand = chooseBuildingJobCommand;
            _extractResourceFieldCommand = extractResourceFieldCommand;
            _goToBuildingPageCommand = goToBuildingPageCommand;
            _upgradeCommand = upgradeCommand;
            _buildingRepository = buildingRepository;
            _constructCommand = constructCommand;
            _chromeManager = chromeManager;
            _checkResourceCommand = checkResourceCommand;
            _addCroplandCommand = addCroplandCommand;
        }

        public override async Task<Result> Execute()
        {
            while (true)
            {
                if (CancellationToken.IsCancellationRequested) return new Cancel();
                var result = await _chooseBuildingJobCommand.Execute(VillageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                var job = _chooseBuildingJobCommand.Value;
                if (job.Type == JobTypeEnums.ResourceBuild)
                {
                    result = await _extractResourceFieldCommand.Execute(VillageId, job);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    continue;
                }

                var plan = JsonSerializer.Deserialize<NormalBuildPlan>(job.Content);
                result = await _goToBuildingPageCommand.Execute(AccountId, VillageId, plan);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

                result = await _checkResourceCommand.Execute(AccountId, VillageId, plan);
                if (result.IsFailed)
                {
                    if (result.HasError<FreeCrop>())
                    {
                        result = await _addCroplandCommand.Execute(VillageId);
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                        continue;
                    }
                    else if (result.HasError<GranaryLimit>())
                    {
                        return result.WithError(new Stop("Please take a look on building job queue")).WithError(new Trace(Trace.TraceMessage()));
                    }
                    else if (result.HasError<WarehouseLimit>())
                    {
                        return result.WithError(new Stop("Please take a look on building job queue")).WithError(new Trace(Trace.TraceMessage()));
                    }
                    var timeResult = await GetEnoughResourcesTime(AccountId, VillageId, plan);
                    if (timeResult.IsFailed)
                    {
                        return timeResult.WithError(new Trace(Trace.TraceMessage()));
                    }
                    return result.WithError(new Trace(Trace.TraceMessage()));
                }

                if (await IsUpgradeable(plan))
                {
                    result = await _upgradeCommand.Execute(AccountId, plan);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
                else
                {
                    result = await _constructCommand.Execute(AccountId, plan);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
            }
        }

        protected override Task SetName()
        {
            _name = "Upgrade building";
            return Task.CompletedTask;
        }

        private async Task<bool> IsUpgradeable(NormalBuildPlan plan)
        {
            var building = await _buildingRepository.GetBasedOnLocation(VillageId, plan.Location);
            if (building.Type == BuildingEnums.Site) return false;
            return true;
        }

        private async Task<Result> GetEnoughResourcesTime(int accountId, int villageId, NormalBuildPlan plan)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;

            HtmlNode node;
            var building = await _buildingRepository.GetBasedOnLocation(villageId, plan.Location);
            if (building.Type == BuildingEnums.Site)
            {
                node = html.GetElementbyId($"contract_building{(int)plan.Type}");
            }
            else
            {
                node = html.GetElementbyId("contract");
            }
            if (node is null) return Result.Fail(new Retry("Cannot read when resource enough [0]"));
            node = node.Descendants("div")
                .FirstOrDefault(x => x.HasClass("errorMessage"));
            if (node is null) return Result.Fail(new Retry("Cannot read when resource enough [1]"));
            node = node.Descendants("span")
                .FirstOrDefault(x => x.HasClass("timer"));
            if (node is null) return Result.Fail(new Retry("Cannot read when resource enough [2]"));
            var time = node.GetAttributeValue("value", 0);
            ExecuteAt = DateTime.Now.Add(TimeSpan.FromSeconds(time + 10));
            return Result.Ok();
        }
    }
}