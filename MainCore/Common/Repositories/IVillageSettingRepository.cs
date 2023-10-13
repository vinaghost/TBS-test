using MainCore.Common.Enums;
using MainCore.Infrasturecture.Persistence;

namespace MainCore.Common.Repositories
{
    public interface IVillageSettingRepository
    {
        Task CheckSetting(int villageId, AppDbContext context);

        Task<Dictionary<VillageSettingEnums, int>> Get(int villageId);

        Task<bool> GetBoolSetting(int villageId, VillageSettingEnums setting);

        Task<int> GetSetting(int villageId, VillageSettingEnums setting);

        Task<int> GetSetting(int villageId, VillageSettingEnums settingMin, VillageSettingEnums settingMax);

        Task Set(int villageId, Dictionary<VillageSettingEnums, int> settings);
    }
}