﻿using MainCore.Entities;

namespace MainCore.Infrasturecture.Services
{
    public interface ITimerManager
    {
        void Shutdown();

        void Start(AccountId accountId);
    }
}