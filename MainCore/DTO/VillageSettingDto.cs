using MainCore.Common.Enums;
using MainCore.Entities;
using Riok.Mapperly.Abstractions;

namespace MainCore.DTO
{
    public class VillageSettingDto
    {
        public int Id { get; set; }
        public VillageSettingEnums Setting { get; set; }
        public int Value { get; set; }
    }

    [Mapper]
    public partial class VillageSettingMapper
    {
        public partial VillageSettingDto Map(VillageSetting dto);

        public VillageSetting Map(VillageId villageId, VillageSettingDto dto)
        {
            var entity = Map(dto);
            entity.VillageId = villageId;
            return entity;
        }

        private partial VillageSetting Map(VillageSettingDto dto);
    }
}