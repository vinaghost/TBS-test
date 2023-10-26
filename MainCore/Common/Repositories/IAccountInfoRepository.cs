using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IAccountInfoRepository
    {
        bool IsPlusActive(AccountId accountId);
        Task Update(AccountId accountId, AccountInfoDto dto);
    }
}