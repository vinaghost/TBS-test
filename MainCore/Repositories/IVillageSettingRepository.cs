using MainCore.Common.Enums;
using MainCore.Entities;

namespace MainCore.Repositories
{
    public interface IVillageSettingRepository
    {
        bool GetBooleanByName(VillageId villageId, VillageSettingEnums setting);

        int GetByName(VillageId villageId, VillageSettingEnums setting);

        int GetByName(VillageId villageId, VillageSettingEnums settingMin, VillageSettingEnums settingMax);
    }
}