using MainCore.Entities;

namespace MainCore.Repositories
{
    public interface IAccountRepository
    {
        string GetPasswordById(AccountId accountId);

        string GetUsernameById(AccountId accountId);
    }
}