using MediatR;

namespace MainCore.Common.Notification
{
    public class AccountInfoUpdated : INotification
    {
        public int AccountId { get; }

        public AccountInfoUpdated(int accountId)
        {
            AccountId = accountId;
        }
    }
}