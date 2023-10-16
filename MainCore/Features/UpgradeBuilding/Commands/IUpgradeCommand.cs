﻿using FluentResults;
using MainCore.Common.Models;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    public interface IUpgradeCommand
    {
        Task<Result> Execute(int accountId, NormalBuildPlan plan);
    }
}