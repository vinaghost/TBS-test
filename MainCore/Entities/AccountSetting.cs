﻿using MainCore.Common.Enums;

namespace MainCore.Entities
{
    public class AccountSetting
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public AccountSettingEnums Setting { get; set; }
        public int Value { get; set; }
    }
}