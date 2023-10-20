using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Common.Repositories
{
    [RegisterAsSingleton]
    public class AccountInfoRepository : IAccountInfoRepository
    {
        private readonly AppDbContext _context;

        public AccountInfoRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool IsPlusActive(int accountId)
        {
           
            var accountInfo = _context.AccountsInfo
                .FirstOrDefault(x => x.AccountId == accountId);
            if (accountInfo is null) return false;
            return accountInfo.HasPlusAccount;
        }

        public void Update(int accountId, AccountInfo accountInfo)
        {
           

            var dbAccountInfo = _context.AccountsInfo
                .FirstOrDefault(x => x.AccountId == accountId);
            if (dbAccountInfo is null)
            {
                _context.Add(accountInfo);
            }
            else
            {
                dbAccountInfo.Tribe = accountInfo.Tribe;
                dbAccountInfo.Gold = accountInfo.Gold;
                dbAccountInfo.Silver = accountInfo.Silver;
                dbAccountInfo.HasPlusAccount = accountInfo.HasPlusAccount;
                _context.Update(dbAccountInfo);
            }
            _context.SaveChanges();
        }
    }
}