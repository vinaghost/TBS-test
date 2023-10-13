using MainCore.Common.Enums;
using MainCore.Entities;
using Riok.Mapperly.Abstractions;

namespace MainCore.Features.Update.DTO
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
        public AccountInfo Map(int accountId, AccountInfoDto dto)
        {
            var entity = Map(dto);
            entity.AccountId = accountId;
            return entity;
        }

        private partial AccountInfo Map(AccountInfoDto dto);
    }
}