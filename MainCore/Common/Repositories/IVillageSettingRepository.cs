using MainCore.Common.Enums;
using MainCore.Infrasturecture.Persistence;

namespace MainCore.Common.Repositories
{
    public interface IVillageSettingRepository
    {
        void CheckSetting(AppDbContext context, int villageId);

        Dictionary<VillageSettingEnums, int> Get(int villageId);

        bool GetBoolSetting(int villageId, VillageSettingEnums setting);

        int GetSetting(int villageId, VillageSettingEnums setting);

        int GetSetting(int villageId, VillageSettingEnums settingMin, VillageSettingEnums settingMax);

        void Set(int villageId, Dictionary<VillageSettingEnums, int> settings);
    }
}