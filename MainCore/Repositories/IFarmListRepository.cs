using MainCore.Models;

namespace MainCore.Repositories
{
    public interface IFarmListRepository
    {
        event Func<int, Task> FarmListsUpdated;

        Task ActiveFarmList(int farmListId);

        Task<int> CountActiveFarmLists(int accountId);

        Task<List<int>> GetActiveFarmLists(int accountId);

        Task<List<FarmList>> GetList(int accountId);

        Task Update(int accountId, List<FarmList> farmLists);
    }
}