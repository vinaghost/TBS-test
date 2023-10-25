using MainCore.Entities;
using Riok.Mapperly.Abstractions;

namespace MainCore.DTO
{
    public class FarmListDto
    {
        public FarmListId Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    [Mapper]
    public partial class FarmListMapper
    {
        public FarmList Map(AccountId accountId, FarmListDto dto)
        {
            var entity = Map(dto);
            entity.AccountId = accountId;
            return entity;
        }

        [MapperIgnoreSource(nameof(FarmListDto.Id))]
        [MapperIgnoreSource(nameof(FarmListDto.IsActive))]
        public partial void MapToEntity(FarmListDto dto, FarmList entity);

        private partial FarmList Map(FarmListDto dto);
    }

    [Mapper]
    public static partial class FarmListStaticMapper
    {
        public static partial IQueryable<FarmListDto> ProjectToDto(this IQueryable<FarmList> entities);
    }
}