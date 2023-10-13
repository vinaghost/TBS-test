using MainCore.Common.Enums;
using MainCore.Infrasturecture.Persistence;

namespace MainCore.Common.Repositories
{
    public interface IAccountSettingRepository
    {
        Task CheckSetting(int accountId, AppDbContext context);
        Task<Dictionary<AccountSettingEnums, int>> Get(int accountId);
        Task<bool> GetBoolSetting(int accountId, AccountSettingEnums setting);

        Task<int> GetSetting(int accountId, AccountSettingEnums setting);
        Task<int> GetSetting(int accountId, AccountSettingEnums settingMin, AccountSettingEnums settingMax);
        Task Set(int accountId, Dictionary<AccountSettingEnums, int> settings);
    }
}