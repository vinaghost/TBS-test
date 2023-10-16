﻿using FluentResults;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    public interface IUpdateBuildingCommand
    {
        Task<Result> Execute(int accountId, int villageId);
    }
}