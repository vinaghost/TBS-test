using MainCore;
using MainCore.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace WPFUI.Repositories
{
    public class BuildRepository : IBuildRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        private readonly List<BuildingEnums> _availableBuildings;

        public BuildRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            _availableBuildings = new();
            for (var i = BuildingEnums.Sawmill; i <= BuildingEnums.Hospital; i++)
            {
                _availableBuildings.Add(i);
            }
        }

        public List<BuildingEnums> GetAvailableBuildings()
        {
            return _availableBuildings;
        }
    }
}