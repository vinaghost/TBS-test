using MediatR;

namespace MainCore.Common.Notification
{
    public class HeroItemUpdated : INotification
    {
        public int AccountId { get; }

        public HeroItemUpdated(int accountId)
        {
            AccountId = accountId;
        }
    }
}