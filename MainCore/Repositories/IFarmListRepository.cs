using MainCore.Entities;
using MainCore.UI.Models.Output;

namespace MainCore.Repositories
{
    public interface IFarmListRepository
    {
        List<FarmListId> GetActive(AccountId accountId);
        void ChangeActive(FarmListId farmListId);
        int CountActive(AccountId accountId);
        List<ListBoxItem> GetItems(AccountId accountId);
    }
}