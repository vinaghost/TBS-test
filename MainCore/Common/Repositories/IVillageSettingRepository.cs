using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;

namespace MainCore.Common.Repositories
{
    public interface IVillageSettingRepository
    {
        void CheckSetting(AppDbContext context, VillageId villageId);

        Dictionary<VillageSettingEnums, int> Get(VillageId villageId);

        bool GetBoolSetting(VillageId villageId, VillageSettingEnums setting);

        int GetSetting(VillageId villageId, VillageSettingEnums setting);

        int GetSetting(VillageId villageId, VillageSettingEnums settingMin, VillageSettingEnums settingMax);

        void Set(VillageId villageId, Dictionary<VillageSettingEnums, int> settings);
    }
}