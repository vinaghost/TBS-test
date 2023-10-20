﻿using MainCore.Common.Enums;
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
        public QueueBuilding Map(int villageId, QueueBuildingDto dto)
        {
            var entity = Map(dto);
            entity.VillageId = villageId;
            entity.Type = Enum.Parse<BuildingEnums>(dto.Type);
            return entity;
        }

        public void MapToEntity(QueueBuildingDto dto, QueueBuilding entity)
        {
            Map(dto, entity);
            entity.Type = Enum.Parse<BuildingEnums>(dto.Type);
        }

        [MapperIgnoreSource(nameof(QueueBuildingDto.Type))]
        private partial QueueBuilding Map(QueueBuildingDto dto);

        [MapperIgnoreSource(nameof(QueueBuildingDto.Type))]
        private partial void Map(QueueBuildingDto dto, QueueBuilding entity);
    }
}