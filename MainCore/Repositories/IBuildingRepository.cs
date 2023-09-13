using MainCore.Models;

namespace MainCore.Repositories
{
    public interface IBuildingRepository
    {
        event Func<int, Task> BuildingUpdated;

        Task<List<Building>> GetList(int villageId);
        Task Update(int villageId, List<Building> buildings);
    }
}