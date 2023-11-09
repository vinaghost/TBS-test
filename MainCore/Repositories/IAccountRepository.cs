using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Repositories
{
    public interface IAccountRepository
    {
        bool Add(AccountDto dto);
        void Add(List<AccountDetailDto> dtos);
        void Delete(AccountId accountId);
        AccountDto Get(AccountId accountId);
        AccessDto GetAccess(AccountId accountId);
        string GetPassword(AccountId accountId);

        string GetUsername(AccountId accountId);
        void Update(AccountDto dto);
    }
}