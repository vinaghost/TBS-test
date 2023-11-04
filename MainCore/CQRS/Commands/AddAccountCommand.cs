using MainCore.Common.Notification;
using MainCore.DTO;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IUseragentManager _useragentManager;
        private readonly IMediator _mediator;

        public AddAccountCommandHandler(IDbContextFactory<AppDbContext> contextFactory, IMediator mediator, IUseragentManager useragentManager)
        {
            _contextFactory = contextFactory;
            _mediator = mediator;
            _useragentManager = useragentManager;
        }

        public async Task Handle(AddAccountCommand request, CancellationToken cancellationToken)
        {
            var success = await Task.Run(() => Add(request.Account), cancellationToken);
            if (!success) return;
            await _mediator.Publish(new AccountUpdated(), cancellationToken);
        }

        private bool Add(AccountDto dto)
        {
            using var context = _contextFactory.CreateDbContext();

            var isExist = context.Accounts
                .Where(x => x.Username == dto.Username)
                .Where(x => x.Server == dto.Server)
                .Any();

            if (isExist) return false;

            var mapper = new AccountMapper();
            var account = mapper.Map(dto);
            foreach (var access in account.Accesses)
            {
                if (string.IsNullOrEmpty(access.Useragent))
                {
                    access.Useragent = _useragentManager.Get();
                }
            }
            context.Add(account);
            context.SaveChanges();
            context.FillAccountSettings(account.Id);
            return true;
        }
    }
}