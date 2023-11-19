using MainCore.Entities;
using MediatR;

namespace MainCore.Notification.Message
{
    public class FarmListUpdated : INotification
    {
        public AccountId AccountId { get; }

        public FarmListUpdated(AccountId accountId)
        {
            AccountId = accountId;
        }
    }
}