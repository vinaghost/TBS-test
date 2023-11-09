using MainCore.Entities;
using MainCore.Notification;
using MainCore.Repositories;
using MediatR;

namespace MainCore.CQRS.Commands
{
    public class MoveJobCommand : IRequest
    {
        public JobId OldJobId { get; }
        public JobId NewJobId { get; }

        public MoveJobCommand(JobId oldJobId, JobId newJobId)
        {
            OldJobId = oldJobId;
            NewJobId = newJobId;
        }
    }

    public class MoveJobCommandHanlder : IRequestHandler<MoveJobCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public MoveJobCommandHanlder(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(MoveJobCommand request, CancellationToken cancellationToken)
        {
            var villageId = await Task.Run(() => _unitOfWork.JobRepository.Move(request.OldJobId, request.NewJobId), cancellationToken);
            if (villageId == VillageId.Empty) return;
            await _mediator.Publish(new JobUpdated(villageId), cancellationToken);
        }
    }
}