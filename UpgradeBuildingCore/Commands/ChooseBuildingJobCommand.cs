using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Models;
using MainCore.Repositories;

namespace UpgradeBuildingCore.Commands
{
    public class ChooseBuildingJobCommand : IChooseBuildingJobCommand
    {
        private readonly IJobRepository _jobRepository;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IAccountInfoRepository _accountInfoRepository;
        private readonly IVillageSettingRepository _villageSettingRepository;
        public Job Value { get; private set; }

        public ChooseBuildingJobCommand(IJobRepository jobRepository, IBuildingRepository buildingRepository, IAccountInfoRepository accountInfoRepository, IVillageSettingRepository villageSettingRepository)
        {
            _jobRepository = jobRepository;
            _buildingRepository = buildingRepository;
            _accountInfoRepository = accountInfoRepository;
            _villageSettingRepository = villageSettingRepository;
        }

        public async Task<Result> Execute(int accountId, int villageId)
        {
            var countJob = await _jobRepository.CountBuildingJob(villageId);

            if (countJob == 0)
            {
                return Result.Fail(Skip.BuildingJobQueueEmpty);
            }

            var countQueueBuilding = await _buildingRepository.CountQueueBuilding(villageId);

            if (countQueueBuilding == 0)
            {
                var job = await _jobRepository.GetFirstJob(villageId);
                Value = job;
                return Result.Ok();
            }

            var isPlusActive = await _accountInfoRepository.IsPlusActive(accountId);
            var isApplyRomanQueueLogic = await _villageSettingRepository.GetBoolSetting(villageId, VillageSettingEnums.ApplyRomanQueueLogicWhenBuilding);

            if (countQueueBuilding == 1)
            {
                if (isPlusActive)
                {
                    var job = await _jobRepository.GetFirstJob(villageId);
                    Value = job;
                    return Result.Ok();
                }
                if (isApplyRomanQueueLogic)
                {
                    var job = await GetJobBasedOnRomanLogic(villageId, countQueueBuilding);
                    if (job is null) return Result.Fail(BuildingQueue.NotTaskInqueue);
                    Value = job;
                    return Result.Ok();
                }
                return Result.Fail(BuildingQueue.Full);
            }

            if (countQueueBuilding == 2)
            {
                if (isApplyRomanQueueLogic)
                {
                    var job = await GetJobBasedOnRomanLogic(villageId, countQueueBuilding);
                    if (job is null) return Result.Fail(BuildingQueue.NotTaskInqueue);
                    Value = job;
                    return Result.Ok();
                }
                return Result.Fail(BuildingQueue.Full);
            }
            return Result.Fail(BuildingQueue.Full);
        }

        private async Task<Job> GetJobBasedOnRomanLogic(int villageId, int countQueueBuilding)
        {
            var countResourceQueueBuilding = await _buildingRepository.CountResourceQueueBuilding(villageId);
            var countInfrastructureQueueBuilding = countQueueBuilding - countResourceQueueBuilding;
            if (countResourceQueueBuilding > countInfrastructureQueueBuilding)
            {
                return await _jobRepository.GetInfrastructureBuildingJob(villageId);
            }
            else
            {
                return await _jobRepository.GetResourceBuildingJob(villageId);
            }
        }
    }
}