﻿using MainCore.Common.Enums;

namespace MainCore.Entities
{
    public class VillageSetting
    {
        public int Id { get; set; }
        public int VillageId { get; set; }
        public VillageSettingEnums Setting { get; set; }
        public int Value { get; set; }
    }
}