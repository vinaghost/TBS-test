﻿using MainCore.Entities;

namespace MainCore.Common.Tasks
{
    public abstract class AccountTask : TaskBase
    {
        public AccountId AccountId { get; private set; }

        public void Setup(AccountId accountId, CancellationToken cancellationToken = default)
        {
            AccountId = accountId;
            CancellationToken = cancellationToken;
        }
    }
}