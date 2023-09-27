using MainCore.Enums;

namespace MainCore.Repositories
{
    public interface IAccountSettingRepository
    {
        Task CheckSetting(int accountId, AppDbContext context);
        Task<bool> GetBoolSetting(int accountId, AccountSettingEnums setting);

        Task<int> GetSetting(int accountId, AccountSettingEnums setting);
        Task<int> GetSetting(int accountId, AccountSettingEnums settingMin, AccountSettingEnums settingMax);
    }
}