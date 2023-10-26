using MainCore.DTO;
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

        public async Task Update(AccountId accountId, AccountInfoDto dto)
        {
            var dbAccountInfo = await Task.Run(
                _context.AccountsInfo
                    .Where(x => x.AccountId == accountId)
                    .FirstOrDefault);

            var mapper = new AccountInfoMapper();
            if (dbAccountInfo is null)
            {
                var accountInfo = mapper.Map(accountId, dto);
                _context.Add(accountInfo);
            }
            else
            {
                mapper.MapToEntity(dto, dbAccountInfo);
                _context.Update(dbAccountInfo);
            }
            await Task.Run(_context.SaveChanges);
        }
    }
}