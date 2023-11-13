using MainCore.Commands.General;
using MainCore.Commands.Navigate;
using MainCore.Commands.Special;
using MainCore.Commands.Step.TrainTroop;
using MainCore.Commands.Update;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Commands
{
    [RegisterAsTransient]
    public class UnitOfCommand : IUnitOfCommand
    {
        public UnitOfCommand(IDelayClickCommand delayClickCommand, IDelayTaskCommand delayTaskCommand, ISwitchTabCommand switchTabCommand, ISwitchVillageCommand switchVillageCommand, IToBuildingCommand toBuildingCommand, IToDorfCommand toDorfCommand, IToHeroInventoryCommand toHeroInventoryCommand, IUpdateAccountInfoCommand updateAccountInfoCommand, IUpdateDorfCommand updateDorfCommand, IUpdateFarmListCommand updateFarmListCommand, IUpdateHeroItemsCommand updateHeroItemsCommand, IUpdateVillageListCommand updateVillageListCommand, ILoginCommand loginCommand, IGetMaximumTroopCommand updateMaximumTroopCommand, IInputAmountTroopCommand inputAmountTroopCommand)
        {
            DelayClickCommand = delayClickCommand;
            DelayTaskCommand = delayTaskCommand;
            SwitchTabCommand = switchTabCommand;
            SwitchVillageCommand = switchVillageCommand;
            ToBuildingCommand = toBuildingCommand;
            ToDorfCommand = toDorfCommand;
            ToHeroInventoryCommand = toHeroInventoryCommand;
            UpdateAccountInfoCommand = updateAccountInfoCommand;
            UpdateDorfCommand = updateDorfCommand;
            UpdateFarmListCommand = updateFarmListCommand;
            UpdateHeroItemsCommand = updateHeroItemsCommand;
            UpdateVillageListCommand = updateVillageListCommand;
            LoginCommand = loginCommand;
            GetMaximumTroopCommand = updateMaximumTroopCommand;
            InputAmountTroopCommand = inputAmountTroopCommand;
        }

        public IDelayClickCommand DelayClickCommand { get; }
        public IDelayTaskCommand DelayTaskCommand { get; }
        public ISwitchTabCommand SwitchTabCommand { get; }
        public ISwitchVillageCommand SwitchVillageCommand { get; }
        public IToBuildingCommand ToBuildingCommand { get; }
        public IToDorfCommand ToDorfCommand { get; }
        public IToHeroInventoryCommand ToHeroInventoryCommand { get; }
        public IUpdateAccountInfoCommand UpdateAccountInfoCommand { get; }
        public IUpdateDorfCommand UpdateDorfCommand { get; }
        public IUpdateFarmListCommand UpdateFarmListCommand { get; }
        public IUpdateHeroItemsCommand UpdateHeroItemsCommand { get; }
        public IUpdateVillageListCommand UpdateVillageListCommand { get; }
        public ILoginCommand LoginCommand { get; }
        public IGetMaximumTroopCommand GetMaximumTroopCommand { get; }
        public IInputAmountTroopCommand InputAmountTroopCommand { get; }
    }
}