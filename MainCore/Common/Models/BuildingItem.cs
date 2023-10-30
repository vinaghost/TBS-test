﻿using MainCore.Common.Enums;

namespace MainCore.Common.Repositories
{
    public class BuildingItem
    {
        public int Location { get; set; }
        public BuildingEnums Type { get; set; }
        public int Level { get; set; }
    }
}