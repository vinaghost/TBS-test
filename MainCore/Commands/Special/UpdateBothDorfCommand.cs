﻿using FluentResults;
using MainCore.Commands.Base;
using MainCore.Common.Errors;
using MainCore.Entities;
using MainCore.Services;
using MediatR;

namespace MainCore.Commands.Special
{
    public class UpdateBothDorfCommand : ByAccountVillageIdRequestBase, IRequest<Result>
    {
        public UpdateBothDorfCommand(AccountId accountId, VillageId villageId) : base(accountId, villageId)
        {
        }
    }

    public class UpdateBothDorfCommandHandler : IRequestHandler<UpdateBothDorfCommand, Result>
    {
        private readonly IChromeManager _chromeManager;
        private readonly IUnitOfCommand _unitOfCommand;

        public UpdateBothDorfCommandHandler(IChromeManager chromeManager, IUnitOfCommand unitOfCommand)
        {
            _chromeManager = chromeManager;
            _unitOfCommand = unitOfCommand;
        }

        public async Task<Result> Handle(UpdateBothDorfCommand request, CancellationToken cancellationToken)
        {
            return await Execute(request.AccountId, request.VillageId);
        }

        public async Task<Result> Execute(AccountId accountId, VillageId villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var url = chromeBrowser.CurrentUrl;
            Result result;
            if (url.Contains("dorf1"))
            {
                result = await _unitOfCommand.UpdateDorfCommand.Execute(accountId, villageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = _unitOfCommand.ToDorfCommand.Execute(accountId, 2);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _unitOfCommand.UpdateDorfCommand.Execute(accountId, villageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            }
            else if (url.Contains("dorf2"))
            {
                result = await _unitOfCommand.UpdateDorfCommand.Execute(accountId, villageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = _unitOfCommand.ToDorfCommand.Execute(accountId, 1);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _unitOfCommand.UpdateDorfCommand.Execute(accountId, villageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            }
            else
            {
                result = _unitOfCommand.ToDorfCommand.Execute(accountId, 2);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _unitOfCommand.UpdateDorfCommand.Execute(accountId, villageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = _unitOfCommand.ToDorfCommand.Execute(accountId, 1);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _unitOfCommand.UpdateDorfCommand.Execute(accountId, villageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            }

            return Result.Ok();
        }
    }
}