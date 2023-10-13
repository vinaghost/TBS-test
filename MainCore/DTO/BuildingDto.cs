using MainCore.Common.Enums;
using MainCore.Entities;
using Riok.Mapperly.Abstractions;

namespace MainCore.Features.Update.DTO
{
    public class BuildingDto
    {
        public BuildingEnums Type { get; set; }
        public int Level { get; set; }
        public bool IsUnderConstruction { get; set; }
        public int Location { get; set; }
    }

    [Mapper]
    public partial class BuildingMapper
    {
        public Building Map(int villageId, BuildingDto dto)
        {
            var entity = Map(dto);
            entity.VillageId = villageId;
            return entity;
        }

        private partial Building Map(BuildingDto dto);
    }
}