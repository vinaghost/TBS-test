using MainCore.CQRS.Base;
using MainCore.Entities;
using MainCore.Notification;
using MainCore.Repositories;
using MediatR;

namespace MainCore.CQRS.Commands
{
    public class DeleteAccountByIdCommand : ByAccountIdRequestBase, IRequest
    {
        public DeleteAccountByIdCommand(AccountId accountId) : base(accountId)
        {
        }
    }

    public class DeleteAccountByIdCommandHandler : IRequestHandler<DeleteAccountByIdCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public DeleteAccountByIdCommandHandler(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteAccountByIdCommand request, CancellationToken cancellationToken)
        {
            await Task.Run(() => _unitOfWork.AccountRepository.Delete(request.AccountId), cancellationToken);
            await _mediator.Publish(new AccountUpdated(), cancellationToken);
        }
    }
}