using FluentResults;
using MainCore.Common.Models;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Notification;
using MainCore.Repositories;
using MediatR;
using System.Text.Json;

namespace MainCore.Commands.Special
{
    [RegisterAsTransient]
    public class ExtractResourceFieldCommand : IExtractResourceFieldCommand
    {
        private readonly IJobRepository _jobRepository;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IMediator _mediator;

        public ExtractResourceFieldCommand(IJobRepository jobRepository, IBuildingRepository buildingRepository, IMediator mediator)
        {
            _jobRepository = jobRepository;
            _buildingRepository = buildingRepository;
            _mediator = mediator;
        }

        public async Task<Result> Execute(VillageId villageId, JobDto job)
        {
            var resourceBuildPlan = JsonSerializer.Deserialize<ResourceBuildPlan>(job.Content);
            var normalBuildPlan = await Task.Run(() => _buildingRepository.GetNormalBuildPlan(villageId, resourceBuildPlan));
            if (normalBuildPlan is null)
            {
                _jobRepository.Delete(job.Id);
            }
            else
            {
                _jobRepository.AddToTop(villageId, normalBuildPlan);
            }
            await _mediator.Publish(new JobUpdated(villageId));
            return Result.Ok();
        }
    }
}