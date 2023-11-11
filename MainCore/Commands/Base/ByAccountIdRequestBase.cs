﻿using MainCore.Entities;

namespace MainCore.Commands.Base
{
    public class ByAccountIdRequestBase
    {
        public AccountId AccountId { get; }

        public ByAccountIdRequestBase(AccountId accountId)
        {
            AccountId = accountId;
        }
    }
}