using MainCore.Models;

namespace MainCore.Repositories
{
    public interface IAccountInfoRepository
    {
        Task<bool> IsPlusActive(int accountId);
        Task Update(int accountId, AccountInfo accountInfo);
    }
}