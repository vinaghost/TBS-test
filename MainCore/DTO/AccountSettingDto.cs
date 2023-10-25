using MainCore.Common.Enums;
using MainCore.Entities;
using Riok.Mapperly.Abstractions;

namespace MainCore.DTO
{
    public class AccountSettingDto
    {
        public int Id { get; set; }
        public AccountSettingEnums Setting { get; set; }
        public int Value { get; set; }
    }

    [Mapper]
    public partial class AccountSettingMapper
    {
        public AccountSetting Map(AccountId accountId, AccountSettingDto dto)
        {
            var entity = Map(dto);
            entity.AccountId = accountId;
            return entity;
        }

        public partial AccountSettingDto Map(AccountSetting dto);

        private partial AccountSetting Map(AccountSettingDto dto);
    }
}