﻿using FluentResults;
using MainCore.Infrasturecture.Services;

namespace MainCore.Features.Update.Commands
{
    public interface IUpdateQueueBuildingCommand
    {
        Task<Result> Execute(int accountId, int villageId);
        Task<Result> Execute(IChromeBrowser chromeBrowser, int villageId);
    }
}