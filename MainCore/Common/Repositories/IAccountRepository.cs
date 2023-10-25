using FluentResults;
using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IAccountRepository
    {
        Task<Result> Add(AccountDto dto);

        Task AddRange(IEnumerable<AccountDto> dtos);

        Task DeleteById(AccountId accountId);

        Task Edit(AccountDto dto);

        Task<IEnumerable<AccountDto>> GetAll();

        Task<AccountDto> GetById(AccountId accountId);
    }
}