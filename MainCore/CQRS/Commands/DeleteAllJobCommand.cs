using MainCore.Common.Notification;
using MainCore.CQRS.Base;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.CQRS.Commands
{
    public class DeleteAllJobCommand : ByVillageIdRequestBase, IRequest
    {
        public DeleteAllJobCommand(VillageId villageId) : base(villageId)
        {
        }
    }

    public class DeleteAllJobCommandHandler : IRequestHandler<DeleteAllJobCommand>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IMediator _mediator;

        public DeleteAllJobCommandHandler(IDbContextFactory<AppDbContext> contextFactory, IMediator mediator)
        {
            _contextFactory = contextFactory;
            _mediator = mediator;
        }

        public async Task Handle(DeleteAllJobCommand request, CancellationToken cancellationToken)
        {
            await Task.Run(() => Delete(request.VillageId), cancellationToken);
            await _mediator.Publish(new JobUpdated(request.VillageId), cancellationToken);
        }

        private void Delete(VillageId villageId)
        {
            using var context = _contextFactory.CreateDbContext();

            context.Jobs
                .Where(x => x.VillageId == villageId.Value)
                .ExecuteDelete();
        }
    }
}