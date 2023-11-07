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
    public static partial class FarmListMapper
    {
        public static FarmList ToEntity(this FarmListDto dto, AccountId accountId)
        {
            var entity = dto.ToEntity();
            entity.AccountId = accountId.Value;
            return entity;
        }

        [MapperIgnoreSource(nameof(FarmListDto.Id))]
        [MapperIgnoreSource(nameof(FarmListDto.IsActive))]
        public static partial void To(this FarmListDto dto, FarmList entity);

        private static partial FarmList ToEntity(this FarmListDto dto);

        public static partial IQueryable<FarmListDto> ToDto(this IQueryable<FarmList> entities);

        private static FarmListId ToFarmListId(this int value) => new(value);

        private static int ToInt(this FarmListId farmListId) => farmListId.Value;
    }
}