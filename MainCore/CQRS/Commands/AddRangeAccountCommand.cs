using MainCore.Common.Notification;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IUseragentManager _useragentManager;
        private readonly IMediator _mediator;

        public AddRangeAccountCommandHandler(IDbContextFactory<AppDbContext> contextFactory, IMediator mediator, IUseragentManager useragentManager)
        {
            _contextFactory = contextFactory;
            _mediator = mediator;
            _useragentManager = useragentManager;
        }

        public async Task Handle(AddRangeAccountCommand request, CancellationToken cancellationToken)
        {
            await Task.Run(() => AddRange(request.Accounts), cancellationToken);
            await _mediator.Publish(new AccountUpdated(), cancellationToken);
        }

        private void AddRange(List<AccountDetailDto> dtos)
        {
            using var context = _contextFactory.CreateDbContext();

            var accounts = new List<Account>();
            foreach (var dto in dtos)
            {
                var isExist = context.Accounts
                    .AsNoTracking()
                    .Where(x => x.Username == dto.Username)
                    .Where(x => x.Server == dto.Server)
                    .Any();
                if (isExist) continue;
                var account = dto.ToEnitty();
                foreach (var access in account.Accesses)
                {
                    if (string.IsNullOrEmpty(access.Useragent))
                    {
                        access.Useragent = _useragentManager.Get();
                    }
                }
                accounts.Add(account);
            }
            context.AddRange(accounts);
            context.SaveChanges();

            foreach (var account in accounts)
            {
                context.FillAccountSettings(new(account.Id));
            }
        }
    }
}