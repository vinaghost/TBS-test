using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MainCore.Notification;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.CQRS.Commands
{
    public class ActiveFarmListCommand : IRequest
    {
        public AccountId AccountId { get; }
        public FarmListId FarmListId { get; }

        public ActiveFarmListCommand(AccountId accountId, FarmListId farmListId)
        {
            AccountId = accountId;
            FarmListId = farmListId;
        }
    }

    public class ActiveFarmListCommandHandler : IRequestHandler<ActiveFarmListCommand>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IMediator _mediator;

        public ActiveFarmListCommandHandler(IDbContextFactory<AppDbContext> contextFactory, IMediator mediator)
        {
            _contextFactory = contextFactory;
            _mediator = mediator;
        }

        public async Task Handle(ActiveFarmListCommand request, CancellationToken cancellationToken)
        {
            await Task.Run(() => ActiveFarmList(request.FarmListId), cancellationToken);
            await _mediator.Publish(new FarmListUpdated(request.AccountId), cancellationToken);
        }

        public void ActiveFarmList(FarmListId farmListId)
        {
            using var context = _contextFactory.CreateDbContext();
            context.FarmLists
               .Where(x => x.Id == farmListId.Value)
               .ExecuteUpdate(x => x.SetProperty(x => x.IsActive, x => !x.IsActive));
        }
    }
}