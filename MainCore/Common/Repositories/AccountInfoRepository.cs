using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;

namespace MainCore.Common.Repositories
{
    [RegisterAsTransient]
    public class AccountInfoRepository : IAccountInfoRepository
    {
        private readonly AppDbContext _context;

        public AccountInfoRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool IsPlusActive(AccountId accountId)
        {
            var accountInfo = _context.AccountsInfo
                    .FirstOrDefault(x => x.AccountId == accountId);

            if (accountInfo is null) return false;
            return accountInfo.HasPlusAccount;
        }
    }
}