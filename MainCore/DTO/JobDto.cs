using MainCore.Common.Enums;
using MainCore.Entities;
using Riok.Mapperly.Abstractions;

namespace MainCore.Features.Update.DTO
{
    public class JobDto
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public JobTypeEnums Type { get; set; }
        public string Content { get; set; }
    }

    [Mapper]
    public partial class JobMapper
    {
        public Job Map(int villageId, JobDto dto)
        {
            var entity = Map(dto);
            entity.VillageId = villageId;
            return entity;
        }

        public partial JobDto Map(Job dto);

        private partial Job Map(JobDto dto);
    }
}