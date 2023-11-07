using MainCore.CQRS.Base;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.CQRS.Queries
{
    public class GetAccountByIdQuery : ByAccountIdRequestBase, IRequest<AccountDto>
    {
        public GetAccountByIdQuery(AccountId accountId) : base(accountId)
        {
        }
    }

    public class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, AccountDto>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public GetAccountByIdQueryHandler(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<AccountDto> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
        {
            var account = await Task.Run(() => Get(request.AccountId), cancellationToken);
            return account;
        }

        private AccountDto Get(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts
                .AsNoTracking()
                .Where(x => x.Id == accountId.Value)
                .ToDto()
                .FirstOrDefault();
            return account;
        }
    }
}