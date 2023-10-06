using MainCore.Entities;
using Riok.Mapperly.Abstractions;

namespace MainCore.Features.Update.DTO
{
    public class FarmListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    [Mapper]
    public partial class FarmListMapper
    {
        public FarmList Map(int accountId, FarmListDto dto)
        {
            var entity = Map(dto);
            entity.AccountId = accountId;
            return entity;
        }

        private partial FarmList Map(FarmListDto dto);
    }
}