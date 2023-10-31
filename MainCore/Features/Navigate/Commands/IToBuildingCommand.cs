﻿using FluentResults;
using MainCore.Entities;

namespace MainCore.Features.Navigate.Commands
{
    public interface IToBuildingCommand
    {
        Result Execute(AccountId accountId, int location);
    }
}