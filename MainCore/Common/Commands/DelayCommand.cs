using FluentResults;
using MainCore.Common.Queries;
using MediatR;

namespace MainCore.Common.Commands
{
    public class DelayCommand : IRequest<Result>
    {
        public int AccountId { get; }

        public DelayCommand(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class DelayCommandHandler : IRequestHandler<DelayCommand, Result>
    {
        private readonly IMediator _mediator;

        public DelayCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result> Handle(DelayCommand request, CancellationToken cancellationToken)
        {
            var delay = await _mediator.Send(new GetClickDelayQuery(request.AccountId), cancellationToken);
            await Task.Delay(delay, cancellationToken);
            return Result.Ok();
        }
    }
}