using MainCore.Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Entities
{
    [Index(nameof(AccountId), nameof(Setting))]
    public class AccountSetting
    {
        public int Id { get; set; }
        public AccountId AccountId { get; set; }
        public AccountSettingEnums Setting { get; set; }
        public int Value { get; set; }
    }
}