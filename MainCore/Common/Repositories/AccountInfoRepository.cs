using MainCore.Common.Notification;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Common.Repositories
{
    [RegisterAsTransient]
    public class AccountInfoRepository : IAccountInfoRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IMediator _mediator;

        public AccountInfoRepository(IDbContextFactory<AppDbContext> contextFactory, IMediator mediator)
        {
            _contextFactory = contextFactory;
            _mediator = mediator;
        }

        public bool IsPlusActive(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var accountInfo = context.AccountsInfo
                    .FirstOrDefault(x => x.AccountId == accountId);

            if (accountInfo is null) return false;
            return accountInfo.HasPlusAccount;
        }

        public async Task Update(AccountId accountId, AccountInfoDto dto)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var dbAccountInfo = await Task.Run(
                    context.AccountsInfo
                        .Where(x => x.AccountId == accountId)
                        .FirstOrDefault);

                var mapper = new AccountInfoMapper();
                if (dbAccountInfo is null)
                {
                    var accountInfo = mapper.Map(accountId, dto);
                    context.Add(accountInfo);
                }
                else
                {
                    mapper.MapToEntity(dto, dbAccountInfo);
                    context.Update(dbAccountInfo);
                }
                await Task.Run(context.SaveChanges);
            }
            await _mediator.Publish(new AccountInfoUpdated(accountId));
        }
    }
}