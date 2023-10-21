namespace MainCore.Common.Repositories
{
    public interface IAccountInfoRepository
    {
        bool IsPlusActive(int accountId);
    }
}