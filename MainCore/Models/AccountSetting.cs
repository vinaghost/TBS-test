using MainCore.Enums;

namespace MainCore.Models
{
    public class AccountSetting
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public AccountSettingEnums Setting { get; set; }
        public int Value { get; set; }
    }
}