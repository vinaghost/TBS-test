using System.Threading.Tasks;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs
{
    public class FarmingViewModel : AccountTabViewModelBase
    {
        protected override Task Load(int accountId)
        {
            return Task.CompletedTask;
        }
    }
}