using MainCore.CQRS.Base;
using MainCore.Entities;
using MainCore.Notification;
using MainCore.Repositories;
using MediatR;

namespace MainCore.CQRS.Commands
{
    public class DeleteAllJobCommand : ByVillageIdRequestBase, IRequest
    {
        public DeleteAllJobCommand(VillageId villageId) : base(villageId)
        {
        }
    }

    public class DeleteAllJobCommandHandler : IRequestHandler<DeleteAllJobCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public DeleteAllJobCommandHandler(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteAllJobCommand request, CancellationToken cancellationToken)
        {
            await Task.Run(() => _unitOfWork.JobRepository.Delete(request.VillageId), cancellationToken);
            await _mediator.Publish(new JobUpdated(request.VillageId), cancellationToken);
        }
    }
}