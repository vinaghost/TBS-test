﻿using FluentResults;
using MainCore.Services;

namespace UpdateCore.Commands
{
    public interface IUpdateInfrastructureCommand
    {
        Task<Result> Execute(int accountId, int villageId);
        Task<Result> Execute(IChromeBrowser chromeBrowser, int villageId);
    }
}