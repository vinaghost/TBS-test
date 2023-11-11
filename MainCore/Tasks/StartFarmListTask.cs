﻿using FluentResults;
using MainCore.Commands.Navigate;
using MainCore.Commands.Special;
using MainCore.Common.Errors;
using MainCore.Common.Tasks;
using MainCore.Infrasturecture.AutoRegisterDi;
using MediatR;

namespace MainCore.Tasks
{
    [RegisterAsTransient(withoutInterface: true)]
    public class StartFarmListTask : AccountTask
    {
        private readonly IMediator _mediator;

        public StartFarmListTask(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<Result> Execute()
        {
            Result result;
            result = await _mediator.Send(new ToFarmListPageCommand(AccountId));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = await _mediator.Send(new StartFarmListCommand(AccountId));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }

        protected override void SetName()
        {
            _name = "Start farm list";
        }
    }
}