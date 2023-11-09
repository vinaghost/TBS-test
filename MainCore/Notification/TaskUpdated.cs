using MainCore.Entities;
using MediatR;

namespace MainCore.Notification
{
    public class TaskUpdated : INotification
    {
        public AccountId AccountId { get; }

        public TaskUpdated(AccountId accountId)
        {
            AccountId = accountId;
        }
    }
}