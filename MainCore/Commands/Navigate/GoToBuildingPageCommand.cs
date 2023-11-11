﻿using FluentResults;
using MainCore.Common.Enums;
using MainCore.Common.Errors;
using MainCore.Common.Models;
using MainCore.Repositories;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Commands.Navigate
{
    [RegisterAsTransient]
    public class GoToBuildingPageCommand : IGoToBuildingPageCommand
    {
        private readonly IToBuildingCommand _toBuildingCommand;
        private readonly ISwitchTabCommand _switchTabCommand;
        private readonly IBuildingRepository _buildingRepository;

        public GoToBuildingPageCommand(IToBuildingCommand toBuildingCommand, IBuildingRepository buildingRepository, ISwitchTabCommand switchTabCommand)
        {
            _toBuildingCommand = toBuildingCommand;
            _buildingRepository = buildingRepository;
            _switchTabCommand = switchTabCommand;
        }

        public Result Execute(AccountId accountId, VillageId villageId, NormalBuildPlan plan)
        {
            Result result;
            result = _toBuildingCommand.Execute(accountId, plan.Location);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            var building = _buildingRepository.GetBuilding(villageId, plan.Location);
            if (building.Type == BuildingEnums.Site)
            {
                var tabIndex = GetBuildingsCategory(building.Type);
                result = _switchTabCommand.Execute(accountId, tabIndex);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            }
            else
            {
                if (building.Level == 0) return Result.Ok();
                if (!HasMultipleTabs(building.Type)) return Result.Ok();
                result = _switchTabCommand.Execute(accountId, 0);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            }
            return Result.Ok();
        }

        private static int GetBuildingsCategory(BuildingEnums building) => building switch
        {
            BuildingEnums.GrainMill => 2,
            BuildingEnums.Sawmill => 2,
            BuildingEnums.Brickyard => 2,
            BuildingEnums.IronFoundry => 2,
            BuildingEnums.Bakery => 2,
            BuildingEnums.Barracks => 1,
            BuildingEnums.HerosMansion => 1,
            BuildingEnums.Academy => 1,
            BuildingEnums.Smithy => 1,
            BuildingEnums.Stable => 1,
            BuildingEnums.GreatBarracks => 1,
            BuildingEnums.GreatStable => 1,
            BuildingEnums.Workshop => 1,
            BuildingEnums.TournamentSquare => 1,
            BuildingEnums.Trapper => 1,
            _ => 0,
        };

        private static bool HasMultipleTabs(BuildingEnums building) => building switch
        {
            BuildingEnums.RallyPoint => true,
            BuildingEnums.CommandCenter => true,
            BuildingEnums.Residence => true,
            BuildingEnums.Palace => true,
            BuildingEnums.Marketplace => true,
            BuildingEnums.Treasury => true,
            _ => false,
        };
    }
}