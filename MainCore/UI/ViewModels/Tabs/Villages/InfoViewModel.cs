using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.UI.ViewModels.Abstract;

namespace MainCore.UI.ViewModels.Tabs.Villages
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class InfoViewModel : VillageTabViewModelBase
    {
        protected override Task Load(int villageId)
        {
            return Task.CompletedTask;
        }
    }
}