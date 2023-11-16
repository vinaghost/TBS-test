﻿using FluentResults;
using MainCore.Common.Errors;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.Repositories;

namespace MainCore.Commands.General
{
    [RegisterAsTransient]
    public class WorkCommand : IWorkCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IUnitOfRepository _unitOfRepository;

        public WorkCommand(IChromeManager chromeManager, IUnitOfRepository unitOfRepository)
        {
            _chromeManager = chromeManager;
            _unitOfRepository = unitOfRepository;
        }

        public Result Execute(AccountId accountId, AccessDto access)
        {
            var chromeBrowser = _chromeManager.Get(accountId);

            var account = _unitOfRepository.AccountRepository.Get(accountId);
            var result = chromeBrowser.Setup(account, access);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }
    }
}