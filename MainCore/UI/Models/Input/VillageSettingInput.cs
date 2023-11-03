using MainCore.Common.Enums;
using MainCore.UI.ViewModels.Abstract;
using MainCore.UI.ViewModels.UserControls;
using ReactiveUI;

namespace MainCore.UI.Models.Input
{
    public class VillageSettingInput : ViewModelBase
    {
        private bool _useHeroResourceForBuilding;
        private bool _applyRomanQueueLogicWhenBuilding;
        private bool _useSpecialUpgrade;

        public TribeSelectorViewModel Tribe { get; } = new();

        private bool _trainTroopEnable;
        private bool _trainWhenLowResource;

        public TroopSelectorViewModel BarrackTroop { get; } = new();
        public TroopSelectorViewModel StableTroop { get; } = new();
        public TroopSelectorViewModel WorkshopTroop { get; } = new();

        public RangeInputViewModel TrainTroopRepeatTime { get; } = new();
        public RangeInputViewModel BarrackAmount { get; } = new();
        public RangeInputViewModel StableAmount { get; } = new();
        public RangeInputViewModel WorkshopAmount { get; } = new();

        public void Set(Dictionary<VillageSettingEnums, int> settings)
        {
            UseHeroResourceForBuilding = settings.GetValueOrDefault(VillageSettingEnums.UseHeroResourceForBuilding) == 1;
            ApplyRomanQueueLogicWhenBuilding = settings.GetValueOrDefault(VillageSettingEnums.ApplyRomanQueueLogicWhenBuilding) == 1;
            UseSpecialUpgrade = settings.GetValueOrDefault(VillageSettingEnums.UseSpecialUpgrade) == 1;

            TrainTroopEnable = settings.GetValueOrDefault(VillageSettingEnums.TrainTroopEnable) == 1;
            TrainWhenLowResource = settings.GetValueOrDefault(VillageSettingEnums.TrainWhenLowResource) == 1;
            TrainTroopRepeatTime.Set(settings.GetValueOrDefault(VillageSettingEnums.TrainTroopRepeatTimeMin), settings.GetValueOrDefault(VillageSettingEnums.TrainTroopRepeatTimeMax));
            var tribe = (TribeEnums)settings.GetValueOrDefault(VillageSettingEnums.Tribe);
            Tribe.Set(tribe);
            var barrackTroop = (TroopEnums)settings.GetValueOrDefault(VillageSettingEnums.BarrackTroop);
            BarrackTroop.Set(barrackTroop, BuildingEnums.Barracks, tribe);
            BarrackAmount.Set(settings.GetValueOrDefault(VillageSettingEnums.BarrackAmountMin), settings.GetValueOrDefault(VillageSettingEnums.BarrackAmountMax));
            var stableTroop = (TroopEnums)settings.GetValueOrDefault(VillageSettingEnums.StableTroop);
            StableTroop.Set(stableTroop, BuildingEnums.Stable, tribe);
            StableAmount.Set(settings.GetValueOrDefault(VillageSettingEnums.StableAmountMin), settings.GetValueOrDefault(VillageSettingEnums.StableAmountMax));
            var workshopTroop = (TroopEnums)settings.GetValueOrDefault(VillageSettingEnums.WorkshopTroop);
            WorkshopTroop.Set(workshopTroop, BuildingEnums.Workshop, tribe);
            WorkshopAmount.Set(settings.GetValueOrDefault(VillageSettingEnums.WorkshopAmountMin), settings.GetValueOrDefault(VillageSettingEnums.WorkshopAmountMax));
        }

        public Dictionary<VillageSettingEnums, int> Get()
        {
            var useHeroResourceForBuilding = UseHeroResourceForBuilding ? 1 : 0;
            var applyRomanQueueLogicWhenBuilding = ApplyRomanQueueLogicWhenBuilding ? 1 : 0;
            var useSpecialUpgrade = UseSpecialUpgrade ? 1 : 0;

            var tribe = (int)Tribe.Get();

            var trainTroopEnable = TrainTroopEnable ? 1 : 0;
            var trainWhenLowResource = TrainWhenLowResource ? 1 : 0;
            var (trainTroopRepeatTimeMin, trainTroopRepeatTimeMax) = TrainTroopRepeatTime.Get();
            var barrackTroop = (int)BarrackTroop.Get();
            var (barrackAmountMin, barrackAmountMax) = BarrackAmount.Get();
            var stableTroop = (int)StableTroop.Get();
            var (stableAmountMin, stableAmountMax) = StableAmount.Get();
            var workshopTroop = (int)WorkshopTroop.Get();
            var (workshopAmountMin, workshopAmountMax) = WorkshopAmount.Get();
            var settings = new Dictionary<VillageSettingEnums, int>()
            {
                { VillageSettingEnums.UseHeroResourceForBuilding, useHeroResourceForBuilding },
                { VillageSettingEnums.ApplyRomanQueueLogicWhenBuilding, applyRomanQueueLogicWhenBuilding },
                { VillageSettingEnums.UseSpecialUpgrade, useSpecialUpgrade },
                { VillageSettingEnums.Tribe, tribe },
                { VillageSettingEnums.TrainTroopEnable, trainTroopEnable },
                { VillageSettingEnums.TrainWhenLowResource, trainWhenLowResource },
                { VillageSettingEnums.TrainTroopRepeatTimeMin, trainTroopRepeatTimeMin },
                { VillageSettingEnums.TrainTroopRepeatTimeMax, trainTroopRepeatTimeMax },
                { VillageSettingEnums.BarrackTroop, barrackTroop },
                { VillageSettingEnums.BarrackAmountMin, barrackAmountMin },
                { VillageSettingEnums.BarrackAmountMax, barrackAmountMax },
                { VillageSettingEnums.StableTroop, stableTroop },
                { VillageSettingEnums.StableAmountMin, stableAmountMin },
                { VillageSettingEnums.StableAmountMax, stableAmountMax },
                { VillageSettingEnums.WorkshopTroop, workshopTroop },
                { VillageSettingEnums.WorkshopAmountMin, workshopAmountMin },
                { VillageSettingEnums.WorkshopAmountMax, workshopAmountMax },
            };
            return settings;
        }

        public bool UseHeroResourceForBuilding
        {
            get => _useHeroResourceForBuilding;
            set => this.RaiseAndSetIfChanged(ref _useHeroResourceForBuilding, value);
        }

        public bool ApplyRomanQueueLogicWhenBuilding
        {
            get => _applyRomanQueueLogicWhenBuilding;
            set => this.RaiseAndSetIfChanged(ref _applyRomanQueueLogicWhenBuilding, value);
        }

        public bool UseSpecialUpgrade
        {
            get => _useSpecialUpgrade;
            set => this.RaiseAndSetIfChanged(ref _useSpecialUpgrade, value);
        }

        public bool TrainTroopEnable
        {
            get => _trainTroopEnable;
            set => this.RaiseAndSetIfChanged(ref _trainTroopEnable, value);
        }

        public bool TrainWhenLowResource
        {
            get => _trainWhenLowResource;
            set => this.RaiseAndSetIfChanged(ref _trainWhenLowResource, value);
        }
    }
}