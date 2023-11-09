using MainCore.Entities;
using MainCore.Notification;
using MainCore.Repositories;
using MediatR;

namespace MainCore.CQRS.Commands
{
    public class DeleteJobByIdCommand : IRequest
    {
        public JobId JobId { get; }

        public DeleteJobByIdCommand(JobId jobId)
        {
            JobId = jobId;
        }
    }

    public class DeleteJobByIdCommandHanlder : IRequestHandler<DeleteJobByIdCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public DeleteJobByIdCommandHanlder(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteJobByIdCommand request, CancellationToken cancellationToken)
        {
            var villageId = await Task.Run(() => _unitOfWork.JobRepository.Delete(request.JobId), cancellationToken);
            if (villageId == VillageId.Empty) return;
            await _mediator.Publish(new JobUpdated(villageId), cancellationToken);
        }
    }
}