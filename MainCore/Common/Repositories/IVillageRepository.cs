using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IVillageRepository
    {
        Village Get(int villageId);

        int GetActive(int accountId);

        List<int> GetInactive(int accountId);

        List<VillageDto> GetList(int accountId);

        List<int> GetUnloadList(int accountId);

        List<Village> Update(int accountId, List<Village> villages);
    }
}