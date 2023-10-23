using FluentResults;
using MainCore.DTO;

namespace MainCore.Common.Repositories
{
    public interface IAccountRepository
    {
        Task<Result> Add(AccountDto dto);

        Task AddRange(IEnumerable<AccountDto> dtos);

        Task DeleteById(int accountId);

        Task Edit(AccountDto dto);

        Task<IEnumerable<AccountDto>> GetAll();

        Task<AccountDto> GetById(int accountId);
    }
}