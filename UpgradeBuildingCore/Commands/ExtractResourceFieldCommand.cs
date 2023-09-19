using FluentResults;
using MainCore.Models;
using MainCore.Models.Plans;
using MainCore.Repositories;
using System.Text.Json;

namespace UpgradeBuildingCore.Commands
{
    public class ExtractResourceFieldCommand : IExtractResourceFieldCommand
    {
        private readonly IJobRepository _jobRepository;
        private readonly IBuildingRepository _buildingRepository;

        public ExtractResourceFieldCommand(IJobRepository jobRepository, IBuildingRepository buildingRepository)
        {
            _jobRepository = jobRepository;
            _buildingRepository = buildingRepository;
        }

        public async Task<Result> Execute(int villageId, Job job)
        {
            var resourceBuildPlan = JsonSerializer.Deserialize<ResourceBuildPlan>(job.Content);
            var normalBuildPlan = await _buildingRepository.GetNormalBuildPlan(villageId, resourceBuildPlan);
            await _jobRepository.Lock(villageId);
            if (normalBuildPlan is null)
            {
                await _jobRepository.Delete(job.Id);
                await _jobRepository.CompleteDelete(villageId, job);
            }
            else
            {
                var newJob = await _jobRepository.AddToTop(villageId, normalBuildPlan);
                await _jobRepository.CompleteAdd(villageId, newJob);
            }
            return Result.Ok();
        }
    }
}