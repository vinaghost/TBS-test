using MainCore.Entities;
using MediatR;

namespace MainCore.Notification
{
    public class AccountSettingUpdated : INotification
    {
        public AccountId AccountId { get; }

        public AccountSettingUpdated(AccountId accountId)
        {
            AccountId = accountId;
        }
    }
}