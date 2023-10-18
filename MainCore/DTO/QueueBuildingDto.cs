using MainCore.Common.Enums;
using MainCore.Entities;
using Riok.Mapperly.Abstractions;

namespace MainCore.DTO
{
    public class QueueBuildingDto
    {
        public int Position { get; set; }
        public int Location { get; set; } = -1;
        public string Type { get; set; }
        public int Level { get; set; }
        public DateTime CompleteTime { get; set; }
    }

    [Mapper]
    public partial class QueueBuildingMapper
    {
        public QueueBuilding Map(int villageId, BuildingEnums type, QueueBuildingDto dto)
        {
            var entity = Map(dto);
            entity.VillageId = villageId;
            entity.Type = type;
            return entity;
        }

        [MapperIgnoreSource(nameof(QueueBuildingDto.Type))]
        private partial QueueBuilding Map(QueueBuildingDto dto);
    }
}