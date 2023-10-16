using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IFarmListRepository
    {
        void ActiveFarmList(int farmListId);

        int CountActiveFarmLists(int accountId);

        List<int> GetActiveFarmLists(int accountId);

        List<FarmList> GetList(int accountId);

        void Update(int accountId, List<FarmList> farmLists);
    }
}