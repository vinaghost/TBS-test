﻿using FluentResults;
using MainCore.Common.Models;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.Parsers;
using MainCore.Repositories;

namespace MainCore.Commands.Special
{
    [RegisterAsTransient]
    public class GetRequiredResourceCommand : IGetRequiredResourceCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IUnitOfParser _unitOfParser;
        private readonly IUnitOfRepository _unitOfRepository;

        public GetRequiredResourceCommand(IChromeManager chromeManager, IUnitOfParser unitOfParser, IUnitOfRepository unitOfRepository)
        {
            _chromeManager = chromeManager;
            _unitOfParser = unitOfParser;
            _unitOfRepository = unitOfRepository;
        }

        public long[] Value { get; private set; }

        public Result Execute(AccountId accountId, VillageId villageId, NormalBuildPlan plan)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;

            var isEmptySite = _unitOfRepository.BuildingRepository.IsEmptySite(villageId, plan.Location);
            Value = _unitOfParser.UpgradeBuildingParser.GetRequiredResource(html, isEmptySite, plan.Type);

            return Result.Ok();
        }
    }
}