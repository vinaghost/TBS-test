using MainCore.Entities;

namespace MainCore.UI.ViewModels.Tabs.Villages
{
    public interface IBuildViewModel
    {
        Task BuildingListRefresh(VillageId villageId);
        Task JobListRefresh(VillageId villageId);
    }
}