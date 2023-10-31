using FluentResults;
using MainCore.Common.Errors;
using MainCore.Repositories;
using MainCore.Common.Tasks;
using MainCore.Features.Navigate.Commands;
using MainCore.Features.Update.Commands;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MediatR;

namespace MainCore.Features.Update.Tasks
{
    [RegisterAsTransient(withoutInterface: true)]
    public class UpdateVillageTask : VillageTask
    {
        private readonly IVillageRepository _villageRepository;
        private readonly ISwitchVillageCommand _switchVillageCommand;
        private readonly IToDorfCommand _toDorfCommand;
        private readonly IChromeManager _chromeManager;
        private readonly IMediator _mediator;

        public UpdateVillageTask(IMediator mediator, IChromeManager chromeManager, IToDorfCommand toDorfCommand, ISwitchVillageCommand switchVillageCommand, IVillageRepository villageRepository)
        {
            _mediator = mediator;
            _chromeManager = chromeManager;
            _toDorfCommand = toDorfCommand;
            _switchVillageCommand = switchVillageCommand;
            _villageRepository = villageRepository;
        }

        public override async Task<Result> Execute()
        {
            if (CancellationToken.IsCancellationRequested) return new Cancel();
            Result result;
            result = _switchVillageCommand.Execute(AccountId, VillageId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            var chromeBrowser = _chromeManager.Get(AccountId);
            var url = chromeBrowser.CurrentUrl;
            if (url.Contains("dorf1"))
            {
                result = await _mediator.Send(new UpdateDorfCommand(AccountId, VillageId));
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = _toDorfCommand.Execute(AccountId, 2);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _mediator.Send(new UpdateDorfCommand(AccountId, VillageId));
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            }
            else if (url.Contains("dorf2"))
            {
                result = await _mediator.Send(new UpdateDorfCommand(AccountId, VillageId));
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = _toDorfCommand.Execute(AccountId, 1);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _mediator.Send(new UpdateDorfCommand(AccountId, VillageId));
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            }
            else
            {
                result = _toDorfCommand.Execute(AccountId, 2);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _mediator.Send(new UpdateDorfCommand(AccountId, VillageId));
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = _toDorfCommand.Execute(AccountId, 1);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _mediator.Send(new UpdateDorfCommand(AccountId, VillageId));
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            }

            return Result.Ok();
        }

        protected override void SetName()
        {
            var village = _villageRepository.GetVillageName(VillageId);
            _name = $"Update buildings in {village}";
        }
    }
}