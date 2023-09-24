using FluentResults;
using MainCore.Errors;
using MainCore.Models;
using MainCore.Repositories;

namespace UpgradeBuildingCore.Commands
{
    public class ChooseBuildingJobCommand : IChooseBuildingJobCommand
    {
        private readonly IJobRepository _jobRepository;
        private readonly IBuildingRepository _buildingRepository;

        public Job Value { get; private set; }

        public ChooseBuildingJobCommand(IJobRepository jobRepository, IBuildingRepository buildingRepository)
        {
            _jobRepository = jobRepository;
            _buildingRepository = buildingRepository;
        }

        public async Task<Result> Execute(int villageId)
        {
            var countJob = await _jobRepository.CountBuildingJob(villageId);

            if (countJob == 0)
            {
                return Result.Fail(Skip.BuildingJobQueueEmpty);
            }

            var countQueueBuilding = await _buildingRepository.CountQueueBuilding(villageId);

            if (countQueueBuilding > 1)
            {
                return Result.Fail(BuildingQueue.Full);
            }
            var job = await _jobRepository.GetFirstJob(villageId);
            Value = job;
            return Result.Ok();
        }
    }
}