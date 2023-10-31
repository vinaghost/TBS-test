using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Repositories
{
    public interface IAccountRepository
    {
        Task Add(AccountDto dto);

        Task DeleteById(AccountId accountId);

        Task Edit(AccountDto dto);

        Task<List<AccountDto>> GetAll();

        Task<AccountDto> GetById(AccountId accountId);

        Task<string> GetPasswordById(AccountId accountId);

        Task<string> GetUsernameById(AccountId accountId);

        Task<bool> IsExist(AccountDto dto);
    }
}