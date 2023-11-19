using MainCore.Entities;
using MediatR;

namespace MainCore.Notification.Message
{
    public class AccountInfoUpdated : INotification
    {
        public AccountId AccountId { get; }

        public AccountInfoUpdated(AccountId accountId)
        {
            AccountId = accountId;
        }
    }
}