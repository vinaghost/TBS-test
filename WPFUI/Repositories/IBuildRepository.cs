using MainCore.Enums;
using MainCore.Models.Plans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WPFUI.Repositories
{
    public interface IBuildRepository
    {
        List<BuildingEnums> GetAvailableBuildings();

        Task Validate(int villageId, NormalBuildPlan plan);
    }
}