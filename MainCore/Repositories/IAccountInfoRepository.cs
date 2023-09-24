using MainCore.Models;

namespace MainCore.Repositories
{
    public interface IAccountInfoRepository
    {
        Task Update(int accountId, AccountInfo accountInfo);
    }
}