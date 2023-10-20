using MediatR;

namespace MainCore.Common.Notification
{
    public class VillageUpdated : INotification
    {
        public int AccountId { get; }

        public VillageUpdated(int accountId)
        {
            AccountId = accountId;
        }
    }
}