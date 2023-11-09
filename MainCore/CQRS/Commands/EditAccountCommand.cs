using MainCore.DTO;
using MainCore.Notification;
using MainCore.Repositories;
using MediatR;

namespace MainCore.CQRS.Commands
{
    public class EditAccountCommand : IRequest
    {
        public AccountDto Account { get; }

        public EditAccountCommand(AccountDto account)
        {
            Account = account;
        }
    }

    public class EditAccountCommandHandler : IRequestHandler<EditAccountCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public EditAccountCommandHandler(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(EditAccountCommand request, CancellationToken cancellationToken)
        {
            await Task.Run(() => _unitOfWork.AccountRepository.Update(request.Account), cancellationToken);
            await _mediator.Publish(new AccountUpdated(), cancellationToken);
        }
    }
}