using FluentResults;
using MainCore.Common.Models;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Notification;
using MainCore.Repositories;
using MediatR;

namespace MainCore.Commands.Special
{
    [RegisterAsTransient]
    public class AddCroplandCommand : IAddCroplandCommand
    {
        private readonly IBuildingRepository _buildingRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IMediator _mediator;

        public AddCroplandCommand(IBuildingRepository buildingRepository, IJobRepository jobRepository, IMediator mediator)
        {
            _buildingRepository = buildingRepository;
            _jobRepository = jobRepository;
            _mediator = mediator;
        }

        public async Task<Result> Execute(VillageId villageId)
        {
            var cropland = _buildingRepository.GetCropland(villageId);

            var plan = new NormalBuildPlan()
            {
                Location = cropland.Location,
                Type = cropland.Type,
                Level = cropland.Level + 1,
            };

            _jobRepository.AddToTop(villageId, plan);
            await _mediator.Publish(new JobUpdated(villageId));
            return Result.Ok();
        }
    }
}