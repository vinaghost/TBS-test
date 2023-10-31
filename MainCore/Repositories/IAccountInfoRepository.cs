using MainCore.Entities;

namespace MainCore.Repositories
{
    public interface IAccountInfoRepository
    {
        bool IsPlusActive(AccountId accountId);
    }
}