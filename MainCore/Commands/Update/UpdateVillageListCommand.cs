﻿using FluentResults;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.Notification;
using MainCore.Parsers;
using MainCore.Repositories;
using MediatR;

namespace MainCore.Commands.Update
{
    [RegisterAsTransient]
    public class UpdateVillageListCommand : IUpdateVillageListCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IMediator _mediator;
        private readonly IUnitOfRepository _unitOfRepository;
        private readonly IUnitOfParser _unitOfParser;

        public UpdateVillageListCommand(IChromeManager chromeManager, IMediator mediator, IUnitOfRepository unitOfRepository, IUnitOfParser unitOfParser)
        {
            _chromeManager = chromeManager;
            _mediator = mediator;
            _unitOfRepository = unitOfRepository;
            _unitOfParser = unitOfParser;
        }

        public async Task<Result> Execute(AccountId accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var dtos = _unitOfParser.VillagePanelParser.Get(html);
            await Task.Run(() => _unitOfRepository.VillageRepository.Update(accountId, dtos.ToList()));
            await _mediator.Publish(new VillageUpdated(accountId));
            return Result.Ok();
        }
    }
}