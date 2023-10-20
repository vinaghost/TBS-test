using MediatR;

namespace MainCore.Features.Update.Trigger
{
    public class HeroItemTrigger : INotification
    {
        public int AccountId { get; }

        public HeroItemTrigger(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class HeroItemTriggerHandler : INotificationHandler<HeroItemTrigger>
    {
        public Task Handle(HeroItemTrigger request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}