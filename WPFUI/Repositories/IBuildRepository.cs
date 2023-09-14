using MainCore.Enums;
using System.Collections.Generic;

namespace WPFUI.Repositories
{
    public interface IBuildRepository
    {
        List<BuildingEnums> GetAvailableBuildings();
    }
}