using MainCore.Entities;
using Riok.Mapperly.Abstractions;

namespace MainCore.DTO
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

    [Mapper]
    public static partial class FarmListStaticMapper
    {
        public static partial IQueryable<FarmListDto> ProjectToDto(this IQueryable<FarmList> entities);
    }
}