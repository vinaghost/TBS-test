using MainCore.Enums;

namespace MainCore.Repositories
{
    public interface IVillageSettingRepository
    {
        Task CheckSetting(int villageId, AppDbContext context);
        Task<bool> GetBoolSetting(int villageId, VillageSettingEnums setting);
        Task<int> GetSetting(int villageId, VillageSettingEnums setting);
        Task<int> GetSetting(int villageId, VillageSettingEnums settingMin, VillageSettingEnums settingMax);
    }
}