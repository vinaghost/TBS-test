using MainCore.Common.Notification;
using MainCore.CQRS.Base;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IMediator _mediator;

        public DeleteAccountByIdCommandHandler(IDbContextFactory<AppDbContext> contextFactory, IMediator mediator)
        {
            _contextFactory = contextFactory;
            _mediator = mediator;
        }

        public async Task Handle(DeleteAccountByIdCommand request, CancellationToken cancellationToken)
        {
            await Task.Run(() => DeleteAccount(request.AccountId), cancellationToken);
            await _mediator.Publish(new AccountUpdated(), cancellationToken);
        }

        private void DeleteAccount(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Accounts
                .Where(x => x.Id == accountId.Value)
                .ExecuteDelete();
        }
    }
}