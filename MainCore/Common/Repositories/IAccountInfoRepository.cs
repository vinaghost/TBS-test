using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IAccountInfoRepository
    {
        Task<bool> IsPlusActive(int accountId);
        Task Update(int accountId, AccountInfo accountInfo);
    }
}