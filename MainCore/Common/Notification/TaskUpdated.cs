using MediatR;

namespace MainCore.Common.Notification
{
    public class TaskUpdated : INotification
    {
        public int AccountId { get; }

        public TaskUpdated(int accountId)
        {
            AccountId = accountId;
        }
    }
}