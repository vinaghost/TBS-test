using MainCore.Entities;

namespace MainCore.Repositories
{
    public interface IFarmListRepository
    {
        List<FarmListId> GetActiveFarmLists(AccountId accountId);
    }
}