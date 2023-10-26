﻿using MainCore.Common.Enums;
using MainCore.Entities;
using Riok.Mapperly.Abstractions;

namespace MainCore.DTO
{
    public class JobDto
    {
        public JobId Id { get; set; }
        public int Position { get; set; }
        public JobTypeEnums Type { get; set; }
        public string Content { get; set; }
    }

    [Mapper]
    public partial class JobMapper
    {
        public Job Map(VillageId villageId, JobDto dto)
        {
            var entity = Map(dto);
            entity.VillageId = villageId;
            return entity;
        }

        public partial JobDto Map(Job dto);

        private partial Job Map(JobDto dto);
    }

    [Mapper]
    public static partial class JobStaticMapper
    {
        public static partial IQueryable<JobDto> ProjectToDto(this IQueryable<Job> entities);
    }
}