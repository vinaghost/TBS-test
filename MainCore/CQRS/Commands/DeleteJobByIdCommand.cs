using MainCore.Notification;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.CQRS.Commands
{
    public class DeleteJobByIdCommand : IRequest
    {
        public JobId JobId { get; }

        public DeleteJobByIdCommand(JobId jobId)
        {
            JobId = jobId;
        }
    }

    public class DeleteJobByIdCommandHanlder : IRequestHandler<DeleteJobByIdCommand>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IMediator _mediator;

        public DeleteJobByIdCommandHanlder(IDbContextFactory<AppDbContext> contextFactory, IMediator mediator)
        {
            _contextFactory = contextFactory;
            _mediator = mediator;
        }

        public async Task Handle(DeleteJobByIdCommand request, CancellationToken cancellationToken)
        {
            var villageId = await Task.Run(() => Delete(request.JobId), cancellationToken);
            if (villageId == VillageId.Empty) return;
            await _mediator.Publish(new JobUpdated(villageId), cancellationToken);
        }

        private VillageId Delete(JobId jobId)
        {
            using var context = _contextFactory.CreateDbContext();

            var job = context.Jobs
                .AsNoTracking()
                .Where(x => x.Id == jobId.Value)
                .FirstOrDefault();
            context.Jobs
                .Where(x => x.Id == jobId.Value)
                .ExecuteDelete();

            context.Jobs
                .Where(x => x.VillageId == job.VillageId)
                .Where(x => x.Position > job.Position)
                .ExecuteUpdate(x => x.SetProperty(x => x.Position, x => x.Position - 1));
            return new(job.VillageId);
        }
    }
}