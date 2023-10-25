using MainCore.Common.Enums;
using MainCore.Entities;
using Riok.Mapperly.Abstractions;

namespace MainCore.DTO
{
    public class AccountInfoDto
    {
        public TribeEnums Tribe { get; set; }
        public int Gold { get; set; }
        public int Silver { get; set; }
        public bool HasPlusAccount { get; set; }
    }

    [Mapper]
    public partial class AccountInfoMapper
    {
        public AccountInfo Map(AccountId accountId, AccountInfoDto dto)
        {
            var entity = Map(dto);
            entity.AccountId = accountId;
            return entity;
        }

        public partial void MapToEntity(AccountInfoDto dto, AccountInfo entity);

        private partial AccountInfo Map(AccountInfoDto dto);
    }
}