using MainCore.Common.Notification;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.CQRS.Commands
{
    public class MoveJobCommand : IRequest
    {
        public JobId OldJobId { get; }
        public JobId NewJobId { get; }

        public MoveJobCommand(JobId oldJobId, JobId newJobId)
        {
            OldJobId = oldJobId;
            NewJobId = newJobId;
        }
    }

    public class MoveJobCommandHanlder : IRequestHandler<MoveJobCommand>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IMediator _mediator;

        public MoveJobCommandHanlder(IDbContextFactory<AppDbContext> contextFactory, IMediator mediator)
        {
            _contextFactory = contextFactory;
            _mediator = mediator;
        }

        public async Task Handle(MoveJobCommand request, CancellationToken cancellationToken)
        {
            var villageId = await Task.Run(() => Move(request.OldJobId, request.NewJobId), cancellationToken);
            if (villageId == VillageId.Empty) return;
            await _mediator.Publish(new JobUpdated(villageId), cancellationToken);
        }

        private VillageId Move(JobId oldJobId, JobId newJobId)
        {
            using var context = _contextFactory.CreateDbContext();

            var jobIds = new List<JobId>() { oldJobId, newJobId };

            var jobs = context.Jobs
                .Where(x => jobIds.Contains(x.Id))
                .ToList();
            if (jobs.Count != 2) return VillageId.Empty;

            (jobs[0].Position, jobs[1].Position) = (jobs[1].Position, jobs[0].Position);
            context.UpdateRange(jobs);
            context.SaveChanges();
            return jobs[0].VillageId;
        }
    }
}