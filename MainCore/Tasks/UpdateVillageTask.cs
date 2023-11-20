﻿using FluentResults;
using MainCore.Commands;
using MainCore.Commands.Special;
using MainCore.Common.Enums;
using MainCore.Common.Errors;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Repositories;
using MainCore.Tasks.Base;
using MediatR;

namespace MainCore.Tasks
{
    [RegisterAsTransient(withoutInterface: true)]
    public class UpdateVillageTask : VillageTask
    {
        private readonly IUnitOfRepository _unitOfRepository;
        private readonly IUnitOfCommand _unitOfCommand;
        private readonly IMediator _mediator;

        public UpdateVillageTask(IUnitOfRepository unitOfRepository, IMediator mediator, IUnitOfCommand unitOfCommand)
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

            result = await _mediator.Send(new UpdateDorf1Command(AccountId, VillageId));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            SetNextExecute();
            return Result.Ok();
        }

        private void SetNextExecute()
        {
            var minutes = _unitOfRepository.VillageSettingRepository.GetByName(VillageId, VillageSettingEnums.AutoRefreshMin, VillageSettingEnums.AutoRefreshMax);
            ExecuteAt = DateTime.Now.AddMinutes(minutes);
        }

        protected override void SetName()
        {
            var village = _unitOfRepository.VillageRepository.GetVillageName(VillageId);
            _name = $"Update village in {village}";
        }
    }
}