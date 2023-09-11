using System.Threading.Tasks;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class InfoViewModel : VillageTabViewModelBase
    {
        protected override Task Load(int villageId)
        {
            return Task.CompletedTask;
        }
    }
}