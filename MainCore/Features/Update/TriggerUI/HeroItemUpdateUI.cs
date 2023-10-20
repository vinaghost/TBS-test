using MediatR;

namespace MainCore.Features.Update.TriggerUI
{
    public class HeroItemTriggerUI : IRequest
    {
        public int AccountId { get; }

        public HeroItemTriggerUI(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class HeroItemTriggerUIHandler : IRequestHandler<HeroItemTriggerUI>
    {
        public Task Handle(HeroItemTriggerUI request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}