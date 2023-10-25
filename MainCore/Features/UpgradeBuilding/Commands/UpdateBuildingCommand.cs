using FluentResults;
using MainCore.Common.Errors;
using MainCore.Entities;
using MainCore.Features.Update.Commands;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MediatR;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    [RegisterAsTransient]
    public class UpdateBuildingCommand : IUpdateBuildingCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IMediator _mediator;

        public UpdateBuildingCommand(IMediator mediator, IChromeManager chromeManager)
        {
            _mediator = mediator;
            _chromeManager = chromeManager;
        }

        public async Task<Result> Execute(AccountId accountId, VillageId villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            Result result;
            result = await _mediator.Send(new UpdateDorfCommand(accountId, villageId));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            return Result.Ok();
        }
    }
}