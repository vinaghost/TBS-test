using MainCore.Entities;
using MediatR;

namespace MainCore.Notification.Message
{
    public class VillageUpdated : INotification
    {
        public AccountId AccountId { get; }

        public VillageUpdated(AccountId accountId)
        {
            AccountId = accountId;
        }
    }
}