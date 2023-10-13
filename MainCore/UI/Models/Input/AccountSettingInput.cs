using MainCore.Common.Enums;
using MainCore.UI.ViewModels.Abstract;
using MainCore.UI.ViewModels.UserControls;
using ReactiveUI;
using System.Collections.Generic;

namespace MainCore.UI.Models.Input
{
    public class AccountSettingInput : ViewModelBase
    {
        public RangeInputViewModel ClickDelay { get; } = new();
        public RangeInputViewModel TaskDelay { get; } = new();

        private bool _isAutoLoadVillage;

        public void Set(Dictionary<AccountSettingEnums, int> settings)
        {
            ClickDelay.Set(settings.GetValueOrDefault(AccountSettingEnums.ClickDelayMin), settings.GetValueOrDefault(AccountSettingEnums.ClickDelayMax));
            TaskDelay.Set(settings.GetValueOrDefault(AccountSettingEnums.TaskDelayMin), settings.GetValueOrDefault(AccountSettingEnums.TaskDelayMax));
            IsAutoLoadVillage = settings.GetValueOrDefault(AccountSettingEnums.IsAutoLoadVillage) == 1;
        }

        public Dictionary<AccountSettingEnums, int> Get()
        {
            var (clickDelayMin, clickDelayMax) = ClickDelay.Get();
            var (taskDelayMin, taskDelayMax) = TaskDelay.Get();
            var isAutoLoadVillage = IsAutoLoadVillage ? 1 : 0;

            var settings = new Dictionary<AccountSettingEnums, int>()
            {
                { AccountSettingEnums.ClickDelayMin, clickDelayMin },
                { AccountSettingEnums.ClickDelayMax, clickDelayMax },
                { AccountSettingEnums.TaskDelayMin, taskDelayMin },
                { AccountSettingEnums.TaskDelayMax, taskDelayMax },
                { AccountSettingEnums.IsAutoLoadVillage, isAutoLoadVillage },
            };
            return settings;
        }

        public bool IsAutoLoadVillage
        {
            get => _isAutoLoadVillage;
            set => this.RaiseAndSetIfChanged(ref _isAutoLoadVillage, value);
        }
    }
}