﻿using MainCore.Common.Enums;
using MainCore.Common.Models;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.UI.Models.Output;

namespace MainCore.Repositories
{
    public interface IBuildingRepository
    {
        int CountQueueBuilding(VillageId villageId);

        int CountResourceQueueBuilding(VillageId villageId);

        BuildingDto GetBuilding(VillageId villageId, int location);

        (BuildingEnums, int) GetBuildingInfo(BuildingId buildingId);

        List<ListBoxItem> GetItems(VillageId villageId);

        Building GetCropland(VillageId villageId);

        NormalBuildPlan GetNormalBuildPlan(VillageId villageId, ResourceBuildPlan plan);

        bool IsEmptySite(VillageId villageId, int location);

        bool IsJobComplete(VillageId villageId, JobDto job);

        bool IsRallyPointExists(VillageId villageId);

        List<BuildingItem> GetLevelBuildings(VillageId villageId);

        void Update(VillageId villageId, List<BuildingDto> dtos);
        List<BuildingEnums> GetTrainTroopBuilding(VillageId villageId);
        int GetBuildingLocation(VillageId villageId, BuildingEnums building);
    }
}