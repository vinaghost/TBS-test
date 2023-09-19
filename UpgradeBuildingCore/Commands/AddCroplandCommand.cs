using FluentResults;
using MainCore.Models.Plans;
using MainCore.Repositories;

namespace UpgradeBuildingCore.Commands
{
    public class AddCroplandCommand : IAddCroplandCommand
    {
        private readonly IBuildingRepository _buildingRepository;
        private readonly IJobRepository _jobRepository;

        public AddCroplandCommand(IBuildingRepository buildingRepository, IJobRepository jobRepository)
        {
            _buildingRepository = buildingRepository;
            _jobRepository = jobRepository;
        }

        public async Task<Result> Execute(int villageId)
        {
            var cropland = await _buildingRepository.GetCropland(villageId);

            var plan = new NormalBuildPlan()
            {
                Location = cropland.Location,
                Type = cropland.Type,
                Level = cropland.Level + 1,
            };

            await _jobRepository.Lock(villageId);
            var job = await _jobRepository.AddToTop(villageId, plan);
            await _jobRepository.CompleteAdd(villageId, job);

            return Result.Ok();
        }
    }
}