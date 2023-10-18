using MediatR;

namespace MainCore.Common.Requests
{
    public class HeroItemUpdate : IRequest
    {
        public int AccountId { get; }

        public HeroItemUpdate(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class HeroItemUpdateHandler : IRequestHandler<HeroItemUpdate>
    {
        public Task Handle(HeroItemUpdate request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}