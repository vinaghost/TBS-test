using MainCore.Entities;
using Riok.Mapperly.Abstractions;

namespace MainCore.Features.Update.DTO
{
    public class VillageDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public bool IsActive { get; set; }
        public bool IsUnderAttack { get; set; }
    }

    [Mapper]
    public partial class VillageMapper
    {
        public Village Map(int accountId, VillageDto dto)
        {
            var entity = Map(dto);
            entity.AccountId = accountId;
            return entity;
        }

        private partial Village Map(VillageDto dto);
    }
}