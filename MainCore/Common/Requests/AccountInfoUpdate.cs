using MediatR;

namespace MainCore.Common.Requests
{
    public class AccountInfoUpdate : IRequest
    {
        public int AccountId { get; }

        public AccountInfoUpdate(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class AccountInfoUpdateHandler : IRequestHandler<AccountInfoUpdate>
    {
        public Task Handle(AccountInfoUpdate request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}