using MainCore.DTO;
using MainCore.Notification;
using MainCore.Repositories;
using MediatR;

namespace MainCore.CQRS.Commands
{
    public class AddRangeAccountCommand : IRequest
    {
        public List<AccountDetailDto> Accounts { get; }

        public AddRangeAccountCommand(List<AccountDetailDto> accounts)
        {
            Accounts = accounts;
        }
    }

    public class AddRangeAccountCommandHandler : IRequestHandler<AddRangeAccountCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public AddRangeAccountCommandHandler(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AddRangeAccountCommand request, CancellationToken cancellationToken)
        {
            await Task.Run(() => _unitOfWork.AccountRepository.Add(request.Accounts), cancellationToken);
            await _mediator.Publish(new AccountUpdated(), cancellationToken);
        }
    }
}