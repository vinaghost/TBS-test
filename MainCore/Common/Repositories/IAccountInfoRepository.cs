using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IAccountInfoRepository
    {
        bool IsPlusActive(int accountId);

        void Update(int accountId, AccountInfo accountInfo);
    }
}