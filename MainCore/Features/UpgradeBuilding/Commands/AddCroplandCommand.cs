using FluentResults;
using MainCore.Common.Models;
using MainCore.Common.Repositories;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MediatR;

namespace MainCore.Features.UpgradeBuilding.Commands
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

            await _jobRepository.AddToTop(villageId, plan);
            return Result.Ok();
        }
    }
}