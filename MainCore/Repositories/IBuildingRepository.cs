using MainCore.Models;

namespace MainCore.Repositories
{
    public interface IBuildingRepository
    {
        event Func<int, Task> BuildingUpdated;

        Task Update(int villageId, List<Building> buildings);
    }
}