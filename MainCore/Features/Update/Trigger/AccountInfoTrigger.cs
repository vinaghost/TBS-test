using MediatR;

namespace MainCore.Features.Update.Trigger
{
    public class AccountInfoTrigger : INotification
    {
        public int AccountId { get; }

        public AccountInfoTrigger(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class AccountInfoTriggerHandler : INotificationHandler<AccountInfoTrigger>
    {
        public Task Handle(AccountInfoTrigger request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}