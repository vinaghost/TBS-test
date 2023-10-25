using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;

namespace MainCore.Common.Repositories
{
    public interface IAccountSettingRepository
    {
        void CheckSetting(AppDbContext context, AccountId accountId);

        Dictionary<AccountSettingEnums, int> Get(AccountId accountId);

        bool GetBoolSetting(AccountId accountId, AccountSettingEnums setting);

        int GetSetting(AccountId accountId, AccountSettingEnums setting);

        int GetSetting(AccountId accountId, AccountSettingEnums settingMin, AccountSettingEnums settingMax);

        void Set(AccountId accountId, Dictionary<AccountSettingEnums, int> settings);
    }
}