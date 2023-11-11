using MainCore.Commands.General;
using MainCore.Commands.Navigate;
using MainCore.Commands.Special;
using MainCore.Commands.Update;

namespace MainCore.Commands
{
    public interface IUnitOfCommand
    {
        IDelayClickCommand DelayClickCommand { get; }
        IDelayTaskCommand DelayTaskCommand { get; }
        ISwitchTabCommand SwitchTabCommand { get; }
        ISwitchVillageCommand SwitchVillageCommand { get; }
        IToBuildingCommand ToBuildingCommand { get; }
        IToDorfCommand ToDorfCommand { get; }
        IToHeroInventoryCommand ToHeroInventoryCommand { get; }
        IUpdateAccountInfoCommand UpdateAccountInfoCommand { get; }
        IUpdateDorfCommand UpdateDorfCommand { get; }
        IUpdateFarmListCommand UpdateFarmListCommand { get; }
        IUpdateHeroItemsCommand UpdateHeroItemsCommand { get; }
        IUpdateVillageListCommand UpdateVillageListCommand { get; }
        ILoginCommand LoginCommand { get; }
    }
}