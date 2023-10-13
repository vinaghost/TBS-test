using MainCore.Common.Enums;
using MainCore.UI.ViewModels.Abstract;
using ReactiveUI;
using System.Collections.Generic;

namespace MainCore.UI.Models.Input
{
    public class VillageSettingInput : ViewModelBase
    {
        private bool _useHeroResourceForBuilding;
        private bool _applyRomanQueueLogicWhenBuilding;
        private bool _useSpecialUpgrade;

        public void Set(Dictionary<VillageSettingEnums, int> settings)
        {
            UseHeroResourceForBuilding = settings.GetValueOrDefault(VillageSettingEnums.UseHeroResourceForBuilding) == 1;
            ApplyRomanQueueLogicWhenBuilding = settings.GetValueOrDefault(VillageSettingEnums.ApplyRomanQueueLogicWhenBuilding) == 1;
            UseSpecialUpgrade = settings.GetValueOrDefault(VillageSettingEnums.UseSpecialUpgrade) == 1;
        }

        public Dictionary<VillageSettingEnums, int> Get()
        {
            var useHeroResourceForBuilding = UseHeroResourceForBuilding ? 1 : 0;
            var applyRomanQueueLogicWhenBuilding = ApplyRomanQueueLogicWhenBuilding ? 1 : 0;
            var useSpecialUpgrade = UseSpecialUpgrade ? 1 : 0;

            var settings = new Dictionary<VillageSettingEnums, int>()
            {
                { VillageSettingEnums.UseHeroResourceForBuilding, useHeroResourceForBuilding },
                { VillageSettingEnums.ApplyRomanQueueLogicWhenBuilding, applyRomanQueueLogicWhenBuilding },
                { VillageSettingEnums.UseSpecialUpgrade, useSpecialUpgrade },
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
    }
}