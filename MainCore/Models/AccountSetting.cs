using MainCore.Enums;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Models
{
    [PrimaryKey(nameof(AccountId), nameof(Setting))]
    public class AccountSetting
    {
        public int AccountId { get; set; }
        public AccountSettingEnums Setting { get; set; }
        public int Value { get; set; }
    }
}