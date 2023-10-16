using MainCore.Common.Enums;
using MainCore.Infrasturecture.Persistence;

namespace MainCore.Common.Repositories
{
    public interface IAccountSettingRepository
    {
        void CheckSetting(AppDbContext context, int accountId);

        Dictionary<AccountSettingEnums, int> Get(int accountId);

        bool GetBoolSetting(int accountId, AccountSettingEnums setting);

        int GetSetting(int accountId, AccountSettingEnums setting);

        int GetSetting(int accountId, AccountSettingEnums settingMin, AccountSettingEnums settingMax);

        void Set(int accountId, Dictionary<AccountSettingEnums, int> settings);
    }
}