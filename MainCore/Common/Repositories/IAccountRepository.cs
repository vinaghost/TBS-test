using FluentResults;
using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IAccountRepository
    {
        event Func<Task> AccountTableChanged;

        Task<Result> Add(Account account);

        Task Add(AccountDto dto);

        Task AddRange(List<Account> accounts);
        Task AddRange(List<AccountDto> dtos);
        Task AddRange(List<AccountsDto> dtos);
        Task Delete(int accountId);

        Task Edit(Account account);

        Task Edit(AccountDto dto);

        Task<List<AccountDto>> Get();

        Task<AccountDto> Get(int accountId);
    }
}