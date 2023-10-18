using FluentResults;
using MainCore.Common.Enums;
using MainCore.Common.Errors;
using MainCore.Common.Repositories;
using MainCore.Common.Requests;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MediatR;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    [RegisterAsTransient]
    public class ChooseBuildingJobCommand : IChooseBuildingJobCommand
    {
        private readonly IJobRepository _jobRepository;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IAccountInfoRepository _accountInfoRepository;
        private readonly IVillageSettingRepository _villageSettingRepository;
        private readonly IMediator _mediator;
        public Job Value { get; private set; }

        public ChooseBuildingJobCommand(IJobRepository jobRepository, IBuildingRepository buildingRepository, IAccountInfoRepository accountInfoRepository, IVillageSettingRepository villageSettingRepository, IMediator mediator)
        {
            _jobRepository = jobRepository;
            _buildingRepository = buildingRepository;
            _accountInfoRepository = accountInfoRepository;
            _villageSettingRepository = villageSettingRepository;
            _mediator = mediator;
        }

        public async Task<Result> Execute(int accountId, int villageId)
        {
            do
            {
                var countJob = _jobRepository.CountBuildingJob(villageId);

                if (countJob == 0)
                {
                    return Result.Fail(Skip.BuildingJobQueueEmpty);
                }

                var countQueueBuilding = _buildingRepository.CountQueueBuilding(villageId);

                if (countQueueBuilding == 0)
                {
                    var job = _jobRepository.GetFirstJob(villageId);
                    if (!await Validate(villageId, job)) continue;
                    Value = job;
                    return Result.Ok();
                }

                var isPlusActive = _accountInfoRepository.IsPlusActive(accountId);
                var isApplyRomanQueueLogic = _villageSettingRepository.GetBoolSetting(villageId, VillageSettingEnums.ApplyRomanQueueLogicWhenBuilding);

                if (countQueueBuilding == 1)
                {
                    if (isPlusActive)
                    {
                        var job = _jobRepository.GetFirstJob(villageId);
                        if (!await Validate(villageId, job)) continue;
                        Value = job;
                        return Result.Ok();
                    }
                    if (isApplyRomanQueueLogic)
                    {
                        var job = GetJobBasedOnRomanLogic(villageId, countQueueBuilding);
                        if (job is null) return Result.Fail(BuildingQueue.NotTaskInqueue);
                        if (!await Validate(villageId, job)) continue;
                        Value = job;
                        return Result.Ok();
                    }
                    return Result.Fail(BuildingQueue.Full);
                }

                if (countQueueBuilding == 2)
                {
                    if (isApplyRomanQueueLogic)
                    {
                        var job = GetJobBasedOnRomanLogic(villageId, countQueueBuilding);
                        if (job is null) return Result.Fail(BuildingQueue.NotTaskInqueue);
                        if (!await Validate(villageId, job)) continue;
                        Value = job;
                        return Result.Ok();
                    }
                    return Result.Fail(BuildingQueue.Full);
                }
                return Result.Fail(BuildingQueue.Full);
            }
            while (true);
        }

        private async Task<bool> Validate(int villageId, Job job)
        {
            if (!_buildingRepository.IsJobValid(villageId, job))
            {
                _jobRepository.Delete(job.Id);
                await _mediator.Send(new JobUpdate(villageId));
                return false;
            }
            return true;
        }

        private Job GetJobBasedOnRomanLogic(int villageId, int countQueueBuilding)
        {
            var countResourceQueueBuilding = _buildingRepository.CountResourceQueueBuilding(villageId);
            var countInfrastructureQueueBuilding = countQueueBuilding - countResourceQueueBuilding;
            if (countResourceQueueBuilding > countInfrastructureQueueBuilding)
            {
                return _jobRepository.GetInfrastructureBuildingJob(villageId);
            }
            else
            {
                return _jobRepository.GetResourceBuildingJob(villageId);
            }
        }
    }
}