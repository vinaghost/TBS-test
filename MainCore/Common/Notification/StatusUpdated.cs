using MainCore.Common.Enums;
using MediatR;

namespace MainCore.Common.Notification
{
    public class StatusUpdated : INotification
    {
        public int AccountId { get; }
        public StatusEnums Status { get; }

        public StatusUpdated(int accountId, StatusEnums status)
        {
            AccountId = accountId;
            Status = status;
        }
    }
}