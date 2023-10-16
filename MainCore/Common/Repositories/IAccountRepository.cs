using FluentResults;
using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IAccountRepository
    {
        Result Add(Account account);

        Result Add(AccountDto dto);

        void AddRange(List<Account> accounts);

        void AddRange(List<AccountDto> dtos);

        void AddRange(List<AccountsDto> dtos);

        void Delete(int accountId);

        void Edit(Account account);

        void Edit(AccountDto dto);

        List<AccountDto> Get();

        AccountDto Get(int accountId);
    }
}