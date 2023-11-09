using MainCore.Notification;
using MainCore.DTO;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IUseragentManager _useragentManager;
        private readonly IMediator _mediator;

        public EditAccountCommandHandler(IDbContextFactory<AppDbContext> contextFactory, IMediator mediator, IUseragentManager useragentManager)
        {
            _contextFactory = contextFactory;
            _mediator = mediator;
            _useragentManager = useragentManager;
        }

        public async Task Handle(EditAccountCommand request, CancellationToken cancellationToken)
        {
            await Task.Run(() => Edit(request.Account), cancellationToken);
            await _mediator.Publish(new AccountUpdated(), cancellationToken);
        }

        private void Edit(AccountDto dto)
        {
            using var context = _contextFactory.CreateDbContext();

            var account = dto.ToEntity();
            foreach (var access in account.Accesses)
            {
                if (string.IsNullOrEmpty(access.Useragent))
                {
                    access.Useragent = _useragentManager.Get();
                }
            }
            context.Update(account);
            context.SaveChanges();
        }
    }
}