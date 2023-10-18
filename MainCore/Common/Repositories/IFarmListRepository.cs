using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IFarmListRepository
    {
        void ActiveFarmList(int farmListId);

        int CountActiveFarmLists(int accountId);

        List<int> GetActiveFarmLists(int accountId);

        IEnumerable<FarmListDto> GetList(int accountId);

        void Update(int accountId, List<FarmList> farmLists);
    }
}