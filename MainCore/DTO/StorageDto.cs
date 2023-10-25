using MainCore.Entities;
using Riok.Mapperly.Abstractions;

namespace MainCore.DTO
{
    public class StorageDto
    {
        public long Wood { get; set; }
        public long Clay { get; set; }
        public long Iron { get; set; }
        public long Crop { get; set; }
        public long Warehouse { get; set; }
        public long Granary { get; set; }
        public long FreeCrop { get; set; }
    }

    [Mapper]
    public partial class StorageMapper
    {
        public Storage Map(VillageId villageId, StorageDto dto)
        {
            var entity = Map(dto);
            entity.VillageId = villageId;
            return entity;
        }

        public partial void MapToEntity(StorageDto dto, Storage entity);

        private partial Storage Map(StorageDto dto);
    }
}