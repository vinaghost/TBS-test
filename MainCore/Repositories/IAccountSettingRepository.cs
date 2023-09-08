using MainCore.Enums;

namespace MainCore.Repositories
{
    public interface IAccountSettingRepository
    {
        Task<bool> GetBoolSetting(int accountId, AccountSettingEnums setting);

        Task<int> GetSetting(int accountId, AccountSettingEnums setting);
    }
}