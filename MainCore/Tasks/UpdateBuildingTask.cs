﻿using FluentResults;
using MainCore.Commands;
using MainCore.Commands.Special;
using MainCore.Common.Errors;
using MainCore.Common.Tasks;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Repositories;
using MediatR;

namespace MainCore.Tasks
{
    [RegisterAsTransient(withoutInterface: true)]
    public class UpdateBuildingTask : VillageTask
    {
        private readonly IUnitOfRepository _unitOfRepository;
        private readonly IUnitOfCommand _unitOfCommand;
        private readonly IMediator _mediator;

        public UpdateBuildingTask(IUnitOfRepository unitOfRepository, IMediator mediator, IUnitOfCommand unitOfCommand)
        {
            _unitOfRepository = unitOfRepository;
            _mediator = mediator;
            _unitOfCommand = unitOfCommand;
        }

        public override async Task<Result> Execute()
        {
            if (CancellationToken.IsCancellationRequested) return new Cancel();

            Result result;
            result = _unitOfCommand.SwitchVillageCommand.Execute(AccountId, VillageId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = await _mediator.Send(new UpdateVillageCommand(AccountId, VillageId));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }

        protected override void SetName()
        {
            var village = _unitOfRepository.VillageRepository.GetVillageName(VillageId);
            _name = $"Update buildings in {village}";
        }
    }
}