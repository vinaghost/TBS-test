using MainCore.Common.Enums;
using MainCore.Entities;
using MediatR;

namespace MainCore.Notification
{
    public class StatusUpdated : INotification
    {
        public AccountId AccountId { get; }
        public StatusEnums Status { get; }

        public StatusUpdated(AccountId accountId, StatusEnums status)
        {
            AccountId = accountId;
            Status = status;
        }
    }
}