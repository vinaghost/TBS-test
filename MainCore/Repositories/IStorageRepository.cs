﻿using FluentResults;
using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Repositories
{
    public interface IStorageRepository
    {
        Result IsEnoughResource(VillageId villageId, long[] requiredResource);

        long[] GetMissingResource(VillageId villageId, long[] requiredResource);
        void Update(VillageId villageId, StorageDto dto);
    }
}