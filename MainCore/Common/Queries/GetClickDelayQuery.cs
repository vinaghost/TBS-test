using MainCore.Common.Enums;
using MediatR;

namespace MainCore.Common.Queries
{
    public class GetClickDelayQuery : IRequest<int>
    {
        public int AccountId { get; }

        public GetClickDelayQuery(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class GetClickDelayQueryHandler : IRequestHandler<GetClickDelayQuery, int>
    {
        private readonly IMediator _mediator;

        public GetClickDelayQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<int> Handle(GetClickDelayQuery request, CancellationToken cancellationToken)
        {
            var settingValueMin = await _mediator.Send(new GetAccountSettingQuery(request.AccountId, AccountSettingEnums.ClickDelayMin), cancellationToken);
            var settingValueMax = await _mediator.Send(new GetAccountSettingQuery(request.AccountId, AccountSettingEnums.ClickDelayMax), cancellationToken);
            return Random.Shared.Next(settingValueMin, settingValueMax);
        }
    }
}