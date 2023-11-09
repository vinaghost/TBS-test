using MainCore.CQRS.Base;
using MainCore.Entities;
using MainCore.Notification;
using MainCore.Repositories;
using MediatR;

namespace MainCore.CQRS.Commands
{
    public class AddJobCommand<T> : ByVillageIdRequestBase, IRequest
    {
        public T Content { get; }

        public AddJobCommand(VillageId villageId, T content) : base(villageId)
        {
            Content = content;
        }
    }

    public class AddJobCommandHandler<T> : IRequestHandler<AddJobCommand<T>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public AddJobCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task Handle(AddJobCommand<T> request, CancellationToken cancellationToken)
        {
            await Task.Run(() => _unitOfWork.JobRepository.Add(request.VillageId, request.Content), cancellationToken);
            await _mediator.Publish(new JobUpdated(request.VillageId));
        }
    }
}