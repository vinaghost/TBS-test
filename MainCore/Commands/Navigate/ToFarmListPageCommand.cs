﻿using FluentResults;
using MainCore.Commands.Base;
using MainCore.Common.Errors;
using MainCore.Common.Errors.Farming;
using MainCore.Entities;
using MainCore.Repositories;
using MediatR;

namespace MainCore.Commands.Navigate
{
    public class ToFarmListPageCommand : ByAccountIdRequestBase, IRequest<Result>
    {
        public ToFarmListPageCommand(AccountId accountId) : base(accountId)
        {
        }
    }

    public class ToFarmListPageCommandHandler : IRequestHandler<ToFarmListPageCommand, Result>
    {
        private readonly IUnitOfRepository _unitOfRepository;
        private readonly IUnitOfCommand _unitOfCommand;

        public ToFarmListPageCommandHandler(IUnitOfRepository unitOfRepository, IUnitOfCommand unitOfCommand)
        {
            _unitOfRepository = unitOfRepository;
            _unitOfCommand = unitOfCommand;
        }

        public async Task<Result> Handle(ToFarmListPageCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;
            Result result;
            result = await _unitOfCommand.UpdateVillageListCommand.Execute(accountId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            var rallypointVillageId = _unitOfRepository.VillageRepository.GetVillageHasRallypoint(accountId);
            if (rallypointVillageId == VillageId.Empty) return Result.Fail(new NoRallypoint());

            result = _unitOfCommand.SwitchVillageCommand.Execute(accountId, rallypointVillageId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = _unitOfCommand.ToDorfCommand.Execute(accountId, 2);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = await _unitOfCommand.UpdateDorfCommand.Execute(accountId, rallypointVillageId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = _unitOfCommand.ToBuildingCommand.Execute(accountId, 39);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = _unitOfCommand.SwitchTabCommand.Execute(accountId, 4);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }
    }
}