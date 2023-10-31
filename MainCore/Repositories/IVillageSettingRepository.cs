using MainCore.Common.Enums;
using MainCore.Entities;

namespace MainCore.Repositories
{
    public interface IVillageSettingRepository
    {
        Dictionary<VillageSettingEnums, int> Get(VillageId villageId);

        bool GetBoolSetting(VillageId villageId, VillageSettingEnums setting);

        int GetSetting(VillageId villageId, VillageSettingEnums setting);

        int GetSetting(VillageId villageId, VillageSettingEnums settingMin, VillageSettingEnums settingMax);

        void Set(VillageId villageId, Dictionary<VillageSettingEnums, int> settings);
    }
}