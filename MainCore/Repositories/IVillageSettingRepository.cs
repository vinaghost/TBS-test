﻿using MainCore.Common.Enums;
using MainCore.Entities;

namespace MainCore.Repositories
{
    public interface IVillageSettingRepository
    {
        Dictionary<VillageSettingEnums, int> Get(VillageId villageId);
        bool GetBooleanByName(VillageId villageId, VillageSettingEnums setting);

        int GetByName(VillageId villageId, VillageSettingEnums setting);

        int GetByName(VillageId villageId, VillageSettingEnums settingMin, VillageSettingEnums settingMax);
        void Update(VillageId villageId, Dictionary<VillageSettingEnums, int> settings);
    }
}