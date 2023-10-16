using MainCore.Entities;

namespace MainCore.Common.Repositories
{
    public interface IVillageRepository
    {
        Village Get(int villageId);

        Village GetActive(int accountId);

        List<Village> GetInactive(int accountId);

        List<Village> GetList(int accountId);

        List<Village> GetUnloadList(int accountId);

        List<Village> Update(int accountId, List<Village> villages);
    }
}