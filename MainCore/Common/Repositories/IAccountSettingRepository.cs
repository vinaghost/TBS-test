using MainCore.Common.Enums;
using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IAccountSettingRepository
    {
        Task FillSetting(AccountId accountId);

        Task<bool> GetBooleanByName(AccountId accountId, AccountSettingEnums setting);

        Task<int> GetByName(AccountId accountId, AccountSettingEnums setting);

        Task<int> GetByName(AccountId accountId, AccountSettingEnums settingMin, AccountSettingEnums settingMax);

        Task<Dictionary<AccountSettingEnums, int>> GetDictionary(AccountId accountId);

        Task SetDictionary(AccountId accountId, Dictionary<AccountSettingEnums, int> settings);
    }
}