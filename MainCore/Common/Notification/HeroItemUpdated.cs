using MainCore.Entities;
using MediatR;

namespace MainCore.Common.Notification
{
    public class HeroItemUpdated : INotification
    {
        public AccountId AccountId { get; }

        public HeroItemUpdated(AccountId accountId)
        {
            AccountId = accountId;
        }
    }
}