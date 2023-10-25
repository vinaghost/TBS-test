using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IFarmListRepository
    {
        void ActiveFarmList(FarmListId farmListId);

        int CountActiveFarmLists(AccountId accountId);

        List<FarmListId> GetActiveFarmLists(AccountId accountId);

        IEnumerable<FarmListDto> GetList(AccountId accountId);

        void Update(AccountId accountId, List<FarmList> farmLists);
    }
}