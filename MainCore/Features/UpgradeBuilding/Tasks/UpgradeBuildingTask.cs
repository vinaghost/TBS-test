using FluentResults;
using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Common.Errors;
using MainCore.Common.Errors.Storage;
using MainCore.Common.Models;
using MainCore.Common.Repositories;
using MainCore.Common.Tasks;
using MainCore.Features.Navigate.Commands;
using MainCore.Features.UpgradeBuilding.Commands;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using System.Text.Json;

namespace MainCore.Features.UpgradeBuilding.Tasks
{
    [RegisterAsTransient(withoutInterface: true)]
    public class UpgradeBuildingTask : VillageTask
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
        private readonly IUpgradeAdsCommand _upgradeAdsCommand;
        private readonly IVillageSettingRepository _villageSettingRepository;

        public UpgradeBuildingTask(IChooseBuildingJobCommand chooseBuildingJobCommand, IExtractResourceFieldCommand extractResourceFieldCommand, IGoToBuildingPageCommand goToBuildingPageCommand, IUpgradeCommand upgradeCommand, IBuildingRepository buildingRepository, IConstructCommand constructCommand, IChromeManager chromeManager, ICheckResourceCommand checkResourceCommand, IAddCroplandCommand addCroplandCommand, IUseHeroResourceCommand useHeroResourceCommand, IStorageRepository storageRepository, IUpdateBuildingCommand updateBuildingCommand, IQueueBuildingRepository queueBuildingRepository, ISwitchVillageCommand switchVillageCommand, IVillageSettingRepository villageSettingRepository, IUpgradeAdsCommand upgradeAdsCommand)

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
            _villageSettingRepository = villageSettingRepository;
            _upgradeAdsCommand = upgradeAdsCommand;
        }

        public override async Task<Result> Execute()
        {
            Result result;

            result = await _switchVillageCommand.Execute(AccountId, VillageId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            while (true)
            {
                if (CancellationToken.IsCancellationRequested) return new Cancel();
                result = await _updateBuildingCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

                result = await _chooseBuildingJobCommand.Execute(AccountId, VillageId);
                if (result.IsFailed)
                {
                    if (result.HasError<BuildingQueue>())
                    {
                        SetTimeQueueBuildingComplete(VillageId);
                    }
                    return result.WithError(new TraceMessage(TraceMessage.Line()));
                }
                var job = _chooseBuildingJobCommand.Value;
                if (job.Type == JobTypeEnums.ResourceBuild)
                {
                    result = await _extractResourceFieldCommand.Execute(VillageId, job);
                    if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                    continue;
                }

                var plan = JsonSerializer.Deserialize<NormalBuildPlan>(job.Content);
                result = await _goToBuildingPageCommand.Execute(AccountId, VillageId, plan);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

                result = await _checkResourceCommand.Execute(AccountId, VillageId, plan);
                if (result.IsFailed)
                {
                    if (result.HasError<FreeCrop>())
                    {
                        result = await _addCroplandCommand.Execute(VillageId);
                        if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                        continue;
                    }
                    else if (result.HasError<GranaryLimit>())
                    {
                        return result.WithError(new Stop("Please take a look on building job queue")).WithError(new TraceMessage(TraceMessage.Line()));
                    }
                    else if (result.HasError<WarehouseLimit>())
                    {
                        return result.WithError(new Stop("Please take a look on building job queue")).WithError(new TraceMessage(TraceMessage.Line()));
                    }

                    if (IsUseHeroResource())
                    {
                        var missingResource = _storageRepository.GetMissingResource(VillageId, _checkResourceCommand.Value);
                        var heroResourceResult = await _useHeroResourceCommand.Execute(AccountId, missingResource);
                        if (heroResourceResult.IsFailed)
                        {
                            if (!heroResourceResult.HasError<Retry>())
                            {
                                var timeResult = SetEnoughResourcesTime(AccountId, VillageId, plan);
                                if (timeResult.IsFailed)
                                {
                                    return timeResult.WithError(new TraceMessage(TraceMessage.Line()));
                                }
                            }

                            return heroResourceResult.WithError(new TraceMessage(TraceMessage.Line()));
                        }
                    }
                }

                if (IsUpgradeable(plan))
                {
                    if (IsSpecialUpgrade())
                    {
                        result = await _upgradeAdsCommand.Execute(AccountId, plan);
                        if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                    }
                    else
                    {
                        result = await _upgradeCommand.Execute(AccountId, plan);
                        if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                    }
                }
                else
                {
                    result = await _constructCommand.Execute(AccountId, plan);
                    if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                }
            }
        }

        protected override void SetName()
        {
            _name = "Upgrade building";
        }

        private bool IsUpgradeable(NormalBuildPlan plan)
        {
            var building = _buildingRepository.GetBuilding(VillageId, plan.Location);
            if (building.Type == BuildingEnums.Site) return false;
            return true;
        }

        private bool IsSpecialUpgrade()
        {
            var useSpecialUpgrade = _villageSettingRepository.GetBoolSetting(VillageId, VillageSettingEnums.UseSpecialUpgrade);
            return useSpecialUpgrade;
        }

        private bool IsUseHeroResource()
        {
            var useHeroResource = _villageSettingRepository.GetBoolSetting(VillageId, VillageSettingEnums.UseHeroResourceForBuilding);
            return useHeroResource;
        }

        private Result SetEnoughResourcesTime(int accountId, int villageId, NormalBuildPlan plan)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;

            HtmlNode node;
            var building = _buildingRepository.GetBuilding(villageId, plan.Location);
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

        private void SetTimeQueueBuildingComplete(int villageId)
        {
            var buildingQueue = _queueBuildingRepository.GetFirst(villageId);
            if (buildingQueue is null)
            {
                ExecuteAt = DateTime.Now.AddSeconds(1);
                return;
            }

            ExecuteAt = buildingQueue.CompleteTime.AddSeconds(1);
        }
    }
}