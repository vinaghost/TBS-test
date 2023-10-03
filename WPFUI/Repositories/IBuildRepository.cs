using FluentResults;
using MainCore.Common.Enums;
using MainCore.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using WPFUI.Models.Output;

namespace WPFUI.Repositories
{
    public interface IBuildRepository
    {
        Task<Result> CheckRequirements(int villageId, NormalBuildPlan plan);
        List<BuildingEnums> GetAvailableBuildings();
        Task<List<BuildingItem>> GetBuildingItems(int villageId);
        Task Validate(int villageId, NormalBuildPlan plan);
    }
}