using FluentResults;
using MainCore.Commands.Special;
using MainCore.Common.Errors;
using MainCore.Common.Tasks;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Repositories;
using MediatR;

namespace MainCore.Tasks
{
    [RegisterAsTransient(withoutInterface: true)]
    public class NPCTask : VillageTask
    {
        private readonly IUnitOfRepository _unitOfRepository;
        private readonly IMediator _mediator;

        public NPCTask(IMediator mediator, IUnitOfRepository unitOfRepository)
        {
            _mediator = mediator;
            _unitOfRepository = unitOfRepository;
        }

        public override async Task<Result> Execute()
        {
            if (CancellationToken.IsCancellationRequested) return new Cancel();
            Result result;
            result = await _mediator.Send(new ToNPCPageCommand(AccountId, VillageId));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = await _mediator.Send(new NPCCommand(AccountId, VillageId));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }

        protected override void SetName()
        {
            var village = _unitOfRepository.VillageRepository.GetVillageName(VillageId);
            _name = $"NPC in {village}";
        }
    }
}