using MainCore.DTO;
using MainCore.Notification;
using MainCore.Repositories;
using MediatR;

namespace MainCore.CQRS.Commands
{
    public class AddAccountCommand : IRequest
    {
        public AccountDto Account { get; }

        public AddAccountCommand(AccountDto account)
        {
            Account = account;
        }
    }

    public class AddAccountCommandHandler : IRequestHandler<AddAccountCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public AddAccountCommandHandler(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AddAccountCommand request, CancellationToken cancellationToken)
        {
            var success = await Task.Run(() => _unitOfWork.AccountRepository.Add(request.Account), cancellationToken);
            if (!success) return;
            await _mediator.Publish(new AccountUpdated(), cancellationToken);
        }
    }
}