using MainCore.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WPFUI.Repositories
{
    public interface IAccountSettingRepository
    {
        Task CheckSetting(int accountId);
        Task<Dictionary<AccountSettingEnums, int>> Get(int accountId);
        Task Set(int accountId, Dictionary<AccountSettingEnums, int> settings);
    }
}