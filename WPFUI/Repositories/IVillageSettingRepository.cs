using MainCore.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WPFUI.Repositories
{
    public interface IVillageSettingRepository
    {
        Task<Dictionary<VillageSettingEnums, int>> Get(int villageId);

        Task Set(int villageId, Dictionary<VillageSettingEnums, int> settings);
    }
}