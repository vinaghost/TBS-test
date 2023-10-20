using MediatR;

namespace MainCore.Common.Notification
{
    public class FarmListUpdated : INotification
    {
        public int AccountId { get; }

        public FarmListUpdated(int accountId)
        {
            AccountId = accountId;
        }
    }
}