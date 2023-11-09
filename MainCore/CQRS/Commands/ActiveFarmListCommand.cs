using MainCore.Entities;
using MainCore.Notification;
using MainCore.Repositories;
using MediatR;

namespace MainCore.CQRS.Commands
{
    public class ActiveFarmListCommand : IRequest
    {
        public AccountId AccountId { get; }
        public FarmListId FarmListId { get; }

        public ActiveFarmListCommand(AccountId accountId, FarmListId farmListId)
        {
            AccountId = accountId;
            FarmListId = farmListId;
        }
    }

    public class ActiveFarmListCommandHandler : IRequestHandler<ActiveFarmListCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public ActiveFarmListCommandHandler(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ActiveFarmListCommand request, CancellationToken cancellationToken)
        {
            await Task.Run(() => _unitOfWork.FarmListRepository.ChangeActiveFarmList(request.FarmListId), cancellationToken);
            await _mediator.Publish(new FarmListUpdated(request.AccountId), cancellationToken);
        }
    }
}