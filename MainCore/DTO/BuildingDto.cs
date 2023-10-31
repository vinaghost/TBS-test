﻿using MainCore.Common.Enums;
using MainCore.Entities;
using Riok.Mapperly.Abstractions;

namespace MainCore.DTO
{
    public class BuildingDto
    {
        public BuildingId Id { get; set; }
        public BuildingEnums Type { get; set; }
        public int Level { get; set; }
        public bool IsUnderConstruction { get; set; }
        public int Location { get; set; }
    }

    [Mapper]
    public partial class BuildingMapper
    {
        public Building Map(VillageId villageId, BuildingDto dto)
        {
            var entity = Map(dto);
            entity.VillageId = villageId;
            return entity;
        }

        public partial BuildingDto Map(Building entity);

        public partial void MapToEntity(BuildingDto dto, Building entity);

        private partial Building Map(BuildingDto dto);
    }

    [Mapper]
    public static partial class BuildingStaticMapper
    {
        public static partial IQueryable<BuildingDto> ProjectToDto(this IQueryable<Building> entities);
    }
}