using FluentResults;
using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Errors.Storage;
using MainCore.Models.Plans;
using MainCore.Repositories;
using MainCore.Services;
using MainCore.Tasks;
using NavigateCore.Commands;
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
        private readonly IUseHeroResourceCommand _useHeroResourceCommand;
        private readonly IStorageRepository _storageRepository;
        private readonly IUpdateBuildingCommand _updateBuildingCommand;
        private readonly IQueueBuildingRepository _queueBuildingRepository;
        private readonly ISwitchVillageCommand _switchVillageCommand;

        public NormalUpgradeBuildingTask(IChooseBuildingJobCommand chooseBuildingJobCommand, IExtractResourceFieldCommand extractResourceFieldCommand, IGoToBuildingPageCommand goToBuildingPageCommand, IUpgradeCommand upgradeCommand, IBuildingRepository buildingRepository, IConstructCommand constructCommand, IChromeManager chromeManager, ICheckResourceCommand checkResourceCommand, IAddCroplandCommand addCroplandCommand, IUseHeroResourceCommand useHeroResourceCommand, IStorageRepository storageRepository, IUpdateBuildingCommand updateBuildingCommand, IQueueBuildingRepository queueBuildingRepository, ISwitchVillageCommand switchVillageCommand)
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
            _useHeroResourceCommand = useHeroResourceCommand;
            _storageRepository = storageRepository;
            _updateBuildingCommand = updateBuildingCommand;
            _queueBuildingRepository = queueBuildingRepository;
            _switchVillageCommand = switchVillageCommand;
        }

        public override async Task<Result> Execute()
        {
            Result result;

            result = await _switchVillageCommand.Execute(AccountId, VillageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            while (true)
            {
                if (CancellationToken.IsCancellationRequested) return new Cancel();
                result = await _updateBuildingCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

                result = await _chooseBuildingJobCommand.Execute(AccountId, VillageId);
                if (result.IsFailed)
                {
                    if (result.HasError<BuildingQueue>())
                    {
                        await SetTimeQueueBuildingComplete(VillageId);
                    }
                    return result.WithError(new Trace(Trace.TraceMessage()));
                }
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

                    var missingResource = await _storageRepository.GetMissingResource(VillageId, _checkResourceCommand.Value);
                    var heroResourceResult = await _useHeroResourceCommand.Execute(AccountId, missingResource);
                    if (heroResourceResult.IsFailed)
                    {
                        if (!heroResourceResult.HasError<Retry>())
                        {
                            var timeResult = await SetEnoughResourcesTime(AccountId, VillageId, plan);
                            if (timeResult.IsFailed)
                            {
                                return timeResult.WithError(new Trace(Trace.TraceMessage()));
                            }
                        }

                        return heroResourceResult.WithError(new Trace(Trace.TraceMessage()));
                    }
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
                result = await _updateBuildingCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
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

        private async Task<Result> SetEnoughResourcesTime(int accountId, int villageId, NormalBuildPlan plan)
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

        private async Task SetTimeQueueBuildingComplete(int villageId)
        {
            var buildingQueue = await _queueBuildingRepository.GetFirst(villageId);
            if (buildingQueue is null)
            {
                ExecuteAt = DateTime.Now.AddSeconds(1);
                return;
            }

            ExecuteAt = buildingQueue.CompleteTime.AddSeconds(1);
        }
    }
}