using MediatR;

namespace MainCore.Features.Update.TriggerUI
{
    public class AccountInfoTriggerUI : IRequest
    {
        public int AccountId { get; }

        public AccountInfoTriggerUI(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class AccountInfoTriggerUIHandler : IRequestHandler<AccountInfoTriggerUI>
    {
        public Task Handle(AccountInfoTriggerUI request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}